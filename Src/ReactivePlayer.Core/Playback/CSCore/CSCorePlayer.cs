using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback.CSCore
{
    // TODO: group status events in single subscriptions grouped by status in order to have Status == Loaded => Duration update before Position update & the inverse when Status == Stopped
    // TODO: buffering support, SingleBlockNotificationStream?? Dopamine docet
    // TODO: annotate class behavior
    // TODO: investigate IWaveSource.AppendSource
    // TODO: investigate IWaveSource.GetLength: how does it calculates duration? How accurate is it guaranteed to be?
    public class CSCorePlayer : IObservableAudioPlayer // TODO: learn how to handle IDisposable from outside and in general how to handle interfaces which implementations may or may not be IDisposable
    {
        #region constants & fields

        public static readonly TimeSpan DefaultPositionUpdateInterval = TimeSpan.FromMilliseconds(50); // TODO: benchmark impact with high freq. updates
        // SemaphoreSlim                https://docs.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim?view=netframework-4.7
        // Semaphore vs SemaphoreSlim   https://docs.microsoft.com/en-us/dotnet/standard/threading/semaphore-and-semaphoreslim
        private readonly SemaphoreSlim __playbackActionsSemaphore = new SemaphoreSlim(1, 1);

        private ISoundOut _soundOut;

        #endregion

        #region ctor

        public CSCorePlayer(TimeSpan positionUpdateInterval)
        {
            this.__playerScopeDisposables = new CompositeDisposable();
            this._playbackScopeDisposables = new CompositeDisposable().DisposeWith(this.__playerScopeDisposables);

            // Track location
            this.__trackLocationSubject = new BehaviorSubject<Uri>(null).DisposeWith(this.__playerScopeDisposables);
            this.WhenTrackLocationChanged = this.__trackLocationSubject
                .AsObservable()
                .DistinctUntilChanged();
            // Position
            this.__positionSubject = new BehaviorSubject<TimeSpan?>(this._soundOut?.WaveSource?.GetPosition()).DisposeWith(this.__playerScopeDisposables);
            this.WhenPositionChanged = this.__positionSubject
                .AsObservable()
                .DistinctUntilChanged();
            // Status
            this.__statusSubject = new BehaviorSubject<PlaybackStatus>(PlaybackStatus.None).DisposeWith(this.__playerScopeDisposables);
            this.WhenStatusChanged = this.__statusSubject
                .AsObservable()
                .DistinctUntilChanged();
            // Volume
            this.__volumeSubject = new BehaviorSubject<float>(this._volume).DisposeWith(this.__playerScopeDisposables);
            this.WhenVolumeChanged = this.__volumeSubject.AsObservable().DistinctUntilChanged();

            // position updater
            this.__whenPositionChangedSubscriber = Observable
                  .Interval(positionUpdateInterval, RxApp.TaskpoolScheduler) // TODO: learn about thread pools
                  .Select(_ => this._soundOut?.WaveSource?.GetPosition())
                  .DistinctUntilChanged()
                  .Publish();
            this.__whenPositionChangedSubscriber
                .Subscribe(position => this.__positionSubject.OnNext(position))
                .DisposeWith(this.__playerScopeDisposables);

            // when started/resumed -> start updating position
            this.WhenStatusChanged
                .Where(status => status == PlaybackStatus.Playing)
                .Subscribe(status =>
                    this._whenPositionChangedSubscription = this.__whenPositionChangedSubscriber
                        .Connect()
                        .DisposeWith(this._playbackScopeDisposables))
                .DisposeWith(this.__playerScopeDisposables);

            // stop updating position when getting into a non-seekable status
            this.WhenStatusChanged
                .Where(status => !PlaybackStatusHelper.CanSeekPlaybackStatuses.Contains(status))
                .Subscribe(status => this._whenPositionChangedSubscription?.Dispose())
                .DisposeWith(this.__playerScopeDisposables);

            // stop listening to stopped event when playback ends/is stopped/interrupted
            this.WhenStatusChanged
                .Where(status => PlaybackStatusHelper.StoppedPlaybackStatuses.Contains(status))
                .Subscribe(status => this._whenSoundOutStoppedSubscription?.Dispose())
                .DisposeWith(this.__playerScopeDisposables);

            // when track loaded -> set position to zero
            this.WhenStatusChanged
                .Where(status => status == PlaybackStatus.Loaded)
                .Subscribe(status => this.__positionSubject.OnNext(TimeSpan.Zero))
                .DisposeWith(this.__playerScopeDisposables);

            // when the playback stops (not paused)
            this.WhenStatusChanged
                .Where(status =>
                    status == PlaybackStatus.None
                    || status == PlaybackStatus.Ended
                    || status == PlaybackStatus.Interrupted
                    || status == PlaybackStatus.Exploded)
                .Subscribe(status =>
                {
                    // if Ended naturally, first set position to total duration (pointless but good looking)
                    if (status == PlaybackStatus.Ended)
                        this.__positionSubject.OnNext(this._soundOut?.WaveSource?.GetLength());

                    // then to null in any stop-case
                    this.__positionSubject.OnNext(null);

                    // dispose previous playback resources and reset subscriptions
                    this._playbackScopeDisposables.Clear();
                })
                .DisposeWith(this.__playerScopeDisposables);
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
            this._whenSoundOutStoppedSubscriber = Observable
                 .FromEventPattern<PlaybackStoppedEventArgs>(
                     h => this._soundOut.Stopped += h,
                     h => this._soundOut.Stopped -= h)
                 .Take(1) // TODO: First vs Take(1)
                 .Publish();
            // manual inturruption removes the .Stopped handler before calling .Stop()
            this._whenSoundOutStoppedSubscriber
                 .Subscribe(e => this.__statusSubject.OnNext(e.EventArgs.HasError ? PlaybackStatus.Exploded : PlaybackStatus.Ended))
                 .DisposeWith(this.__playerScopeDisposables);

            #region CAN's

            this.__canLoadSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanLoadPlaybackStatuses.Contains(this.__statusSubject.Value)).DisposeWith(this.__playerScopeDisposables);
            this.WhenCanLoadChanged = this.__canLoadSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanLoadPlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canLoadSubject.OnNext(can))
                .DisposeWith(this.__playerScopeDisposables);

            // Can - Play
            this.__canPlaySubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanPlayPlaybackStatuses.Contains(this.__statusSubject.Value)).DisposeWith(this.__playerScopeDisposables);
            this.WhenCanPlayChanged = this.__canPlaySubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanPlayPlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canPlaySubject.OnNext(can))
                .DisposeWith(this.__playerScopeDisposables);

            // Can - Pause
            this.__canPauseSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(this.__statusSubject.Value)).DisposeWith(this.__playerScopeDisposables);
            this.WhenCanPauseChanged = this.__canPauseSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canPauseSubject.OnNext(can))
                .DisposeWith(this.__playerScopeDisposables);

            // Can - Resume
            this.__canResumeSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(this.__statusSubject.Value)).DisposeWith(this.__playerScopeDisposables);
            this.WhenCanResumeChanged = this.__canResumeSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canResumeSubject.OnNext(can))
                .DisposeWith(this.__playerScopeDisposables);

            // Can - Stop
            this.__canStopSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(this.__statusSubject.Value)).DisposeWith(this.__playerScopeDisposables);
            this.WhenCanStopChanged = this.__canStopSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canStopSubject.OnNext(can))
                .DisposeWith(this.__playerScopeDisposables);

            // Can - Seek
            this.__canSeekSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanSeekPlaybackStatuses.Contains(this.__statusSubject.Value)).DisposeWith(this.__playerScopeDisposables);
            this.WhenCanSeekChanged = this.__canSeekSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Subscribe(status => this.__canSeekSubject.OnNext((this._soundOut?.WaveSource?.CanSeek ?? false) && PlaybackStatusHelper.CanSeekPlaybackStatuses.Contains(status)))
                .DisposeWith(this.__playerScopeDisposables);

            #endregion

            #region logging

            // track location change log
            this.WhenTrackLocationChanged.Subscribe(tl => Debug.WriteLine($"Track\t\t=\t{tl?.ToString() ?? "null"}")).DisposeWith(this.__playerScopeDisposables);
            // position change log
            this.WhenPositionChanged
                .Sample(TimeSpan.FromMilliseconds(500))
                .Subscribe(pos => Debug.WriteLine($"Position\t=\t{pos?.ToString() ?? "null"}")).DisposeWith(this.__playerScopeDisposables);
            // status change log
            this.WhenStatusChanged.Subscribe(status => Debug.WriteLine($"Status\t\t=\t{Enum.GetName(typeof(PlaybackStatus), status)}")).DisposeWith(this.__playerScopeDisposables);
            // duration chage log
            this.WhenDurationChanged.Subscribe(duration => Debug.WriteLine($"Duration\t=\t{duration?.ToString() ?? "null"}")).DisposeWith(this.__playerScopeDisposables);
            // volume change log
            this.WhenVolumeChanged.Throttle(TimeSpan.FromMilliseconds(500)).Subscribe(volume => Debug.WriteLine($"Volume\t\t=\t{volume}")).DisposeWith(this.__playerScopeDisposables);

            #endregion
        }

        public CSCorePlayer() : this(CSCorePlayer.DefaultPositionUpdateInterval) { }

        #endregion

        #region methods

        #region public

        public async Task LoadTrackAsync(Uri trackLocation)
        {
            await this.__playbackActionsSemaphore.WaitAsync();

            try
            {
                if (this.__canLoadSubject.Value)
                {
                    this.__statusSubject.OnNext(PlaybackStatus.Loading); // TODO: notify Loading before or after checking for trackLocation validity?

                    if (WasapiOut.IsSupportedOnCurrentPlatform)
                        this._soundOut = new WasapiOut();
                    else
                        this._soundOut = new DirectSoundOut(100);

                    this._soundOut.Initialize(CodecFactory.Instance.GetCodec(trackLocation));
                    this._soundOut.Volume = this._volume;

                    // register playbackScopeDisposables
                    this._soundOut.WaveSource.DisposeWith(this._playbackScopeDisposables);
                    this._soundOut.DisposeWith(this._playbackScopeDisposables);

                    // start listening to ISoundOut.Stopped
                    this._whenSoundOutStoppedSubscription = this._whenSoundOutStoppedSubscriber.Connect();

                    this.__statusSubject.OnNext(PlaybackStatus.Loaded);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(LoadTrackAsync)}: {ex.Message}");

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

                // ensure resources allocated during the failed loading process are released
                // this might be moved/duped at loading start
                this._playbackScopeDisposables.Clear();

                this.__statusSubject.OnNext(PlaybackStatus.Exploded);

                throw ex;
            }
            finally
            {
                this.__playbackActionsSemaphore.Release();
            }
        }

        public async Task PlayAsync()
        {
            await this.__playbackActionsSemaphore.WaitAsync();

            try
            {
                if (this.__canPlaySubject.Value)
                {
                    this._soundOut.Play();
                    SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Playing);
                    this.__statusSubject.OnNext(PlaybackStatus.Playing);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(PlayAsync)}: {ex.Message}");

                this.__statusSubject.OnNext(PlaybackStatus.Exploded);
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

            try
            {
                if (this.__canPauseSubject.Value)
                {
                    this._soundOut.Pause();
                    SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Paused);
                    this.__statusSubject.OnNext(PlaybackStatus.Paused);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(PauseAsync)}: {ex.Message}");
                this.__statusSubject.OnNext(PlaybackStatus.Exploded);
                //throw;
            }
            finally
            {
                this.__playbackActionsSemaphore.Release();
            }
        }

        public async Task ResumeAsync()
        {
            await this.__playbackActionsSemaphore.WaitAsync();

            try
            {
                if (this.__canResumeSubject.Value)
                {
                    this._soundOut.Resume();
                    SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Playing);
                    this.__statusSubject.OnNext(PlaybackStatus.Playing);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(ResumeAsync)}: {ex.Message}");
                this.__statusSubject.OnNext(PlaybackStatus.Exploded);
                //throw;
            }
            finally
            {
                this.__playbackActionsSemaphore.Release();
            }
        }

        public async Task StopAsync()
        {
            await this.__playbackActionsSemaphore.WaitAsync();

            try
            {
                if (this.__canStopSubject.Value)
                {
                    // stop listening to the Stopped event
                    this._whenSoundOutStoppedSubscription.Dispose();

                    this._soundOut.Stop();
                    this._soundOut.WaitForStopped(); // TODO: consider adding the WaitForStopped( timeout )
                                                     // TODO: compare WaitForStopped which uses a WaitHandle https://github.com/filoe/cscore/blob/29410b12ae35321c4556b072c0711a8f289c0544/CSCore/Extensions.cs#L410 vs SpinWait.SpinUntil
                    SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Stopped);
                    this.__statusSubject.OnNext(PlaybackStatus.Interrupted);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(StopAsync)}: {ex.Message}");
                this.__statusSubject.OnNext(PlaybackStatus.Exploded);
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

        #endregion

        #region observable events

        private readonly BehaviorSubject<Uri> __trackLocationSubject;
        public IObservable<Uri> WhenTrackLocationChanged { get; } // TODO: investigate whether .AsObservable().DistinctUntilChanged() creates a new observable every time someone subscribes

        private readonly IConnectableObservable<TimeSpan?> __whenPositionChangedSubscriber;
        private IDisposable _whenPositionChangedSubscription;
        private ISubject<TimeSpan?> __positionSubject;
        public IObservable<TimeSpan?> WhenPositionChanged { get; }

        public IObservable<TimeSpan?> WhenDurationChanged { get; }

        private IConnectableObservable<EventPattern<PlaybackStoppedEventArgs>> _whenSoundOutStoppedSubscriber;
        private IDisposable _whenSoundOutStoppedSubscription;
        private readonly BehaviorSubject<PlaybackStatus> __statusSubject;
        public IObservable<PlaybackStatus> WhenStatusChanged { get; }

        private readonly BehaviorSubject<bool> __canLoadSubject;
        public IObservable<bool> WhenCanLoadChanged { get; }

        private readonly BehaviorSubject<bool> __canPlaySubject;
        public IObservable<bool> WhenCanPlayChanged { get; }

        private readonly BehaviorSubject<bool> __canPauseSubject;
        public IObservable<bool> WhenCanPauseChanged { get; }

        private readonly BehaviorSubject<bool> __canResumeSubject;
        public IObservable<bool> WhenCanResumeChanged { get; }

        private readonly BehaviorSubject<bool> __canStopSubject;
        public IObservable<bool> WhenCanStopChanged { get; }

        private readonly BehaviorSubject<bool> __canSeekSubject;
        public IObservable<bool> WhenCanSeekChanged { get; }

        private readonly BehaviorSubject<float> __volumeSubject;
        public IObservable<float> WhenVolumeChanged { get; }

        #endregion

        #region IDisposable

        private CompositeDisposable __playerScopeDisposables;
        private CompositeDisposable _playbackScopeDisposables;
        private object __disposingLock = new object();

        public void Dispose() // TODO: review implementation, also consider if there's some Interlocked way to do it
        {
            try
            {
                lock (this.__disposingLock)
                {
                    if (this.__playerScopeDisposables != null && !this.__playerScopeDisposables.IsDisposed)
                    {
                        // TODO: add stop?
                        this.__playerScopeDisposables?.Dispose();
                        this.__playerScopeDisposables = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(Dispose)}: {ex.Message}");
                throw ex;
            }
        }

        #endregion
    }
}