using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.MediaFoundation;
using CSCore.Streams;
using CSCore.Streams.Effects;
using CSCore.Streams.SampleConverter;
using CSCore.Utils;
using CSCore.Utils.Buffer;
using CSCore.Win32;
using CSCore.DSP;
using CSCore.DMO;
using CSCore.DMO.Effects;
using CSCore.DirectSound;
using CSCore.SoundOut;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.App
{
    // TODO: group status events in single subscriptions grouped by status in order to have Status == Loaded => Duration update before Position update & the inverse when Status == Stopped
    // TODO: buffering support, SingleBlockNotificationStream?? Dopamine docet
    // TODO: annotate class behavior
    // TODO: investigate IWaveSource.AppendSource
    // TODO: investigate IWaveSource.GetLength: how does it calculates duration? How accurate is it guaranteed to be?
    public class CSCorePlayer : IObservableAudioPlayer // TODO: learn how to handle IDisposable from outside and in general how to handle interfaces which implementations may or may not be IDisposable
    {
        #region constants & fields

        private static readonly TimeSpan PositionUpdatesInterval = TimeSpan.FromMilliseconds(500); // TODO: benchmark update frequency
        // SemaphoreSlim                https://docs.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim?view=netframework-4.7
        // Semaphore vs SemaphoreSlim   https://docs.microsoft.com/en-us/dotnet/standard/threading/semaphore-and-semaphoreslim
        private readonly SemaphoreSlim __playbackActionsSemaphore = new SemaphoreSlim(1, 1);

        private ISoundOut _soundOut;
        //private bool _isManuallyStopping = false; // Note: used to (and ONLY to) mark the stop event to be handled as manually stopped, set on manual stopping, unset on new playback setup

        //private IObservable<ISoundOut> __whenSundOutChanged;
        //private IObservable<IWaveSource> __whenWaveSourceChanged;

        #endregion

        #region ctor

        public CSCorePlayer()
        {
            this.__playerDisposables = new CompositeDisposable();
            this._playbackDisposables = new CompositeDisposable().DisposeWith(this.__playerDisposables);

            // Track location
            this.__trackLocationSubject = new BehaviorSubject<Uri>(null).DisposeWith(this.__playerDisposables);
            this.WhenTrackLocationChanged = this.__trackLocationSubject
                .AsObservable()
                .DistinctUntilChanged();
            // Position
            this.__positionSubject = new BehaviorSubject<TimeSpan?>(null);
            this.WhenPositionChanged = this.__positionSubject
                .AsObservable()
                .DistinctUntilChanged();
            // Status
            this.__playbackStatusSubject = new BehaviorSubject<PlaybackStatus>(PlaybackStatus.None).DisposeWith(this.__playerDisposables);
            this.WhenStatusChanged = this.__playbackStatusSubject
                .AsObservable()
                .DistinctUntilChanged();
            // Volume
            this.__volumeSubject = new BehaviorSubject<float>(this._volume);
            this.WhenVolumeChanged = this.__volumeSubject.AsObservable().DistinctUntilChanged();

            // SoundOut & SoundOut.WaveSource
            //this.__whenSundOutChanged = this.WhenAnyValue(_ => _._soundOut).DistinctUntilChanged();
            //this.__whenWaveSourceChanged = this.WhenAnyValue(_ => _._soundOut.WaveSource).DistinctUntilChanged();

            // volume
            this.WhenVolumeChanged
                .Throttle(TimeSpan.FromMilliseconds(500)) // only log volume when the new value is maintained for at least 500 ms
                .Subscribe(volume => Debug.WriteLine($"Volume\t\t=\t{volume}"))
                .DisposeWith(this.__playerDisposables);

            // position updater
            this.__whenPositionChangedSubscriber = Observable
                  .Interval(CSCorePlayer.PositionUpdatesInterval, RxApp.TaskpoolScheduler) // TODO: learn about thread pools
                  .Select(_ => this._soundOut?.WaveSource?.GetPosition())
                  .DistinctUntilChanged()
                  .Publish();
            this.__whenPositionChangedSubscriber
                .Subscribe(position => this.__positionSubject.OnNext(position))
                .DisposeWith(this.__playerDisposables);

            // when started/resumed -> start updating position
            this.WhenStatusChanged
                .Where(status => status == PlaybackStatus.Playing)
                .Subscribe(status =>
                    this._whenPositionChangedSubscription = this.__whenPositionChangedSubscriber
                        .Connect()
                        .DisposeWith(this._playbackDisposables))
                .DisposeWith(this.__playerDisposables);

            // stop updating position when getting into a non-seekable status
            this.WhenStatusChanged
                .Where(status => !PlaybackStatusHelper.SeekablePlaybackStatuses.Contains(status))
                .Subscribe(status => this._whenPositionChangedSubscription?.Dispose())
                .DisposeWith(this.__playerDisposables);

            // stop listening to stopped event when playback ends/is stopped/interrupted
            this.WhenStatusChanged
                .Where(status => PlaybackStatusHelper.StoppedPlaybackStatuses.Contains(status))
                .Subscribe(status => this._whenPlaybackStoppedSubscription?.Dispose())
                .DisposeWith(this.__playerDisposables);

            // when track loaded -> set position to zero
            this.WhenStatusChanged
                .Where(status => status == PlaybackStatus.Loaded)
                .Subscribe(status => this.__positionSubject.OnNext(TimeSpan.Zero))
                .DisposeWith(this.__playerDisposables);
            // set position to null when the playback stops (not paused)
            this.WhenStatusChanged
                .Where(status =>
                    status == PlaybackStatus.Ended
                    || status == PlaybackStatus.Interrupted
                    || status == PlaybackStatus.Exploded
                    || status == PlaybackStatus.None)
                .Subscribe(status =>
                {
                    // if Ended naturally, first set position to total duration (pointless but good looking)
                    if (status == PlaybackStatus.Ended)
                        this.__positionSubject.OnNext(this._soundOut?.WaveSource?.GetLength());

                    // then to null in any stop-case
                    this.__positionSubject.OnNext(null);
                })
                .DisposeWith(this.__playerDisposables);
            // Duration
            this.WhenDurationChanged = this.WhenStatusChanged
                .Where(status =>
                    status == PlaybackStatus.None
                    || status == PlaybackStatus.Loaded
                    || status == PlaybackStatus.Ended
                    || status == PlaybackStatus.Interrupted
                    || status == PlaybackStatus.Exploded)
                .Select(status => (status == PlaybackStatus.Loaded) ? this._soundOut?.WaveSource?.GetLength() : null)
                .DistinctUntilChanged();

            // playback stopped subscriber/subscription
            this._whenPlaybackStoppedSubscriber = Observable
                 .FromEventPattern<PlaybackStoppedEventArgs>(
                     h => this._soundOut.Stopped += h,
                     h => this._soundOut.Stopped -= h)
                 .Take(1) // TODO: First vs Take(1)
                 .Publish();
            // manual inturruption removes the .Stopped handler before calling .Stop()
            this._whenPlaybackStoppedSubscriber
                 .Subscribe(e => this.__playbackStatusSubject.OnNext(e.EventArgs.HasError ? PlaybackStatus.Exploded : PlaybackStatus.Ended))
                 .DisposeWith(this.__playerDisposables); // the factory lives as long as the player

            #region CAN's

            // Can - PlayNew
            this.__canPlayNewSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanPlayNewPlaybackStatuses.Contains(this.__playbackStatusSubject.Value));
            this.WhenCanPlayNewChanged = this.__canPlayNewSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanPlayNewPlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canPlayNewSubject.OnNext(can))
                .DisposeWith(this.__playerDisposables);

            // Can - Pause
            this.__canPauseSubject = new BehaviorSubject<bool>(false);
            this.WhenCanPausehanged = this.__canPauseSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canPauseSubject.OnNext(can))
                .DisposeWith(this.__playerDisposables);

            // Can - Resume
            this.__canResumeSubject = new BehaviorSubject<bool>(false);
            this.WhenCanResumeChanged = this.__canResumeSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canResumeSubject.OnNext(can))
                .DisposeWith(this.__playerDisposables);

            // Can - Stop
            this.__canStopSubject = new BehaviorSubject<bool>(false);
            this.WhenCanStophanged = this.__canStopSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canStopSubject.OnNext(can))
                .DisposeWith(this.__playerDisposables);

            // Can - Seek
            this.__canSeekSubject = new BehaviorSubject<bool>(false);
            this.WhenCanSeekChanged = this.__canSeekSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Subscribe(status => this.__canSeekSubject.OnNext((this._soundOut?.WaveSource?.CanSeek ?? false) && PlaybackStatusHelper.SeekablePlaybackStatuses.Contains(status)))
                .DisposeWith(this.__playerDisposables);

            #endregion

            #region logging

            // track location change log
            this.WhenTrackLocationChanged.Subscribe(tl => Debug.WriteLine($"Track\t\t=\t{tl?.ToString() ?? "null"}")).DisposeWith(this.__playerDisposables);
            // position change log
            this.WhenPositionChanged.Subscribe(pos => Debug.WriteLine($"Position\t=\t{pos?.ToString() ?? "null"}")).DisposeWith(this.__playerDisposables);
            // status change log
            this.WhenStatusChanged.Subscribe(status => Debug.WriteLine($"Status\t\t=\t{Enum.GetName(typeof(PlaybackStatus), status)}")).DisposeWith(this.__playerDisposables);
            // duration chage log
            this.WhenDurationChanged.Subscribe(duration => Debug.WriteLine($"Duration\t=\t{duration?.ToString() ?? "null"}")).DisposeWith(this.__playerDisposables);

            #endregion
        }

        #endregion

        #region methods

        #region public

        public async Task PlayNewAsync(Uri trackLocation)
        {
            await this.StopAsync();
            await this.__playbackActionsSemaphore.WaitAsync();

            try
            {
                // dispose previous playback resources and reset subscriptions
                // TODO: this should be moved to stopped events
                this._playbackDisposables.Clear();

                this.__playbackStatusSubject.OnNext(PlaybackStatus.Loading); // TODO: notify Loading before or after checking for trackLocation validity?
                this.InitializeTrack(trackLocation);
                this.__playbackStatusSubject.OnNext(PlaybackStatus.Loaded);

                this._soundOut.Play();
                SpinWait.SpinUntil(() => this._soundOut?.PlaybackState == PlaybackState.Playing);
                this.__playbackStatusSubject.OnNext(PlaybackStatus.Playing);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(PlayNewAsync)}: {ex.Message}");

                switch (ex)
                {
                    case NullReferenceException nse:
                        break;
                    case NotSupportedException nse:
                        break;
                    case ArgumentNullException ane:
                        break;
                    default:
                        break;
                }

                this.__playbackStatusSubject.OnNext(PlaybackStatus.Exploded);
                //throw ex;
            }
            finally
            {
                this.__playbackActionsSemaphore.Release();
            }
        }

        public async Task PauseAsync()
        {
            await this.__playbackActionsSemaphore.WaitAsync();

            if (this.__canPauseSubject.Value)
            {
                this._soundOut?.Pause();
                SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Paused);
                this.__playbackStatusSubject.OnNext(PlaybackStatus.Paused);
            }

            this.__playbackActionsSemaphore.Release();
        }

        public async Task ResumeAsync()
        {
            await this.__playbackActionsSemaphore.WaitAsync();

            if (this.__canResumeSubject.Value)
            {
                this._soundOut?.Resume();
                SpinWait.SpinUntil(() => this._soundOut?.PlaybackState == PlaybackState.Playing);
                this.__playbackStatusSubject.OnNext(PlaybackStatus.Playing);
            }

            this.__playbackActionsSemaphore.Release();
        }

        public async Task StopAsync()
        {
            await this.__playbackActionsSemaphore.WaitAsync();

            try
            {
                if (this.__canStopSubject.Value)
                {
                    //this._isManuallyStopping = true;

                    // stop listening to the Stopped event
                    this._whenPlaybackStoppedSubscription.Dispose();

                    this._soundOut?.Stop();
                    this._soundOut?.WaitForStopped(); // TODO: consider adding the WaitForStopped( timeout )
                                                      // TODO: compare WaitForStopped which uses a WaitHandle https://github.com/filoe/cscore/blob/29410b12ae35321c4556b072c0711a8f289c0544/CSCore/Extensions.cs#L410 vs SpinWait.SpinUntil
                    SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Stopped);
                    this.__playbackStatusSubject.OnNext(PlaybackStatus.Interrupted);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(StopAsync)}: {ex.Message}");

                this.__playbackStatusSubject.OnNext(PlaybackStatus.Exploded);
                //throw;
            }
            finally
            {
                this.__playbackActionsSemaphore.Release();
            }
        }

        public async Task SeekToAsync(TimeSpan position)
        {
            await this.__playbackActionsSemaphore.WaitAsync();

            try
            {
                //if (this._soundOut?.WaveSource?.CanSeek ?? false)
                if (this.__canSeekSubject.Value)
                {
                    if (position < TimeSpan.Zero || position > this._soundOut.WaveSource.GetLength())
                        throw new ArgumentOutOfRangeException(nameof(position), position, $"{nameof(position)} out of {nameof(IWaveSource)} range."); // TODO: localize

                    this._soundOut.WaveSource.SetPosition(position);
                    this.__positionSubject.OnNext(this._soundOut?.WaveSource?.GetPosition());
                    //return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(SetVolume)}: {ex.Message}");
                // swallow (might happen that the ISoundOut is "not initialized yet"
                // TODO: find out why CSCore source code
            }
            finally
            {
                this.__playbackActionsSemaphore.Release();
            }
        }

        private float _volume = 0.5f;
        public void SetVolume(float volume)
        {
            try
            {
                if (this._soundOut != null)
                    this._soundOut.Volume = volume;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(SetVolume)}: {ex.Message}");
                // swallow (might happen that the ISoundOut is "not initialized yet"
                // TODO: find out why CSCore source code
            }
            finally
            {
                this._volume = volume;
                this.__volumeSubject.OnNext(this._volume);
            }
        }

        #endregion

        #region private

        private void InitializeTrack(Uri trackLocation)
        {
            try
            {
                if (WasapiOut.IsSupportedOnCurrentPlatform)
                    this._soundOut = new WasapiOut();
                else
                    this._soundOut = new DirectSoundOut(100);

                this._soundOut.Initialize(CodecFactory.Instance.GetCodec(trackLocation));
                this._soundOut.Volume = this._volume;

                this._soundOut.WaveSource.DisposeWith(this._playbackDisposables);
                this._soundOut.DisposeWith(this._playbackDisposables);

                this._whenPlaybackStoppedSubscription = this._whenPlaybackStoppedSubscriber.Connect();

                //this._isManuallyStopping = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(InitializeTrack)}: {ex.Message}");
                throw ex;
            }
        }

        #endregion

        #endregion

        #region observable events

        private readonly BehaviorSubject<Uri> __trackLocationSubject;
        public IObservable<Uri> WhenTrackLocationChanged { get; } // TODO: investigate whether .AsObservable().DistinctUntilChanged() creates a new observable every time someone subscribes

        private readonly IConnectableObservable<TimeSpan?> __whenPositionChangedSubscriber;
        private IDisposable _whenPositionChangedSubscription;
        private ISubject<TimeSpan?> __positionSubject;
        public IObservable<TimeSpan?> WhenPositionChanged { get; }

        public IObservable<TimeSpan?> WhenDurationChanged { get; }

        private readonly BehaviorSubject<PlaybackStatus> __playbackStatusSubject;
        private IConnectableObservable<EventPattern<PlaybackStoppedEventArgs>> _whenPlaybackStoppedSubscriber;
        private IDisposable _whenPlaybackStoppedSubscription;
        public IObservable<PlaybackStatus> WhenStatusChanged { get; }

        private readonly BehaviorSubject<bool> __canPlayNewSubject;
        public IObservable<bool> WhenCanPlayNewChanged { get; }

        private readonly BehaviorSubject<bool> __canPauseSubject;
        public IObservable<bool> WhenCanPausehanged { get; }

        private readonly BehaviorSubject<bool> __canResumeSubject;
        public IObservable<bool> WhenCanResumeChanged { get; }

        private readonly BehaviorSubject<bool> __canStopSubject;
        public IObservable<bool> WhenCanStophanged { get; } //  => this._canStopSubject.AsObservable();

        private readonly BehaviorSubject<bool> __canSeekSubject;
        public IObservable<bool> WhenCanSeekChanged { get; }

        private readonly BehaviorSubject<float> __volumeSubject;
        public IObservable<float> WhenVolumeChanged { get; }

        #endregion

        #region IDisposable

        private CompositeDisposable __playerDisposables;
        private CompositeDisposable _playbackDisposables;
        private object __disposingLock = new object();

        public void Dispose() // TODO: review implementation, also consider if there's some Interlocked way to do it
        {
            try
            {
                lock (this.__disposingLock)
                {
                    if (this.__playerDisposables != null && !this.__playerDisposables.IsDisposed)
                    {
                        // TODO: add stop?
                        this.__playerDisposables?.Dispose();
                        this.__playerDisposables = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(Dispose)}: {ex.Message}");

                throw;
            }
        }

        #endregion
    }
}