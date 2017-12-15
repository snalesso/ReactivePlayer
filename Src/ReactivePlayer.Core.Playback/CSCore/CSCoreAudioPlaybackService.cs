using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
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
    // TODO: investigate IWaveSource.GetLength: how does it calculate duration? How accurate is it guaranteed to be?
    // TODO: add timeout for track loading?
    // TODO: consider using AsyncLock (System.Reactive.Core)
    public class CSCoreAudioPlaybackService : IPlaybackService // TODO: learn how to handle IDisposable from outside and in general how to handle interfaces which implementations may or may not be IDisposable
    {
        #region constants & fields

        // TODO: benchmark impact high frequency position updates
        private readonly TimeSpan _positionUpdatesInterval = TimeSpan.FromMilliseconds(100);
        // SemaphoreSlim                https://docs.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim?view=netframework-4.7
        // Semaphore vs SemaphoreSlim   https://docs.microsoft.com/en-us/dotnet/standard/threading/semaphore-and-semaphoreslim
        private readonly SemaphoreSlim _playbackActionsSemaphore = new SemaphoreSlim(1, 1);

        private ISoundOut __soundOut;

        #endregion

        #region ctor

        // TODO: add logger
        public CSCoreAudioPlaybackService(/*TimeSpan positionUpdatesInterval = default(TimeSpan)*/)
        {
            this._playerScopeDisposables = new CompositeDisposable();
            this.__playbackScopeDisposables = new CompositeDisposable().DisposeWith(this._playerScopeDisposables);

            // Track location
            this._audioSourceLocationSubject = new BehaviorSubject<Uri>(null).DisposeWith(this._playerScopeDisposables);
            this.WhenAudioSourceLocationChanged = this._audioSourceLocationSubject
                .AsObservable()
                .DistinctUntilChanged();
            // Position
            this._positionSubject = new BehaviorSubject<TimeSpan?>(this.__soundOut?.WaveSource?.GetPosition()).DisposeWith(this._playerScopeDisposables);
            this.WhenPositionChanged = this._positionSubject
                .AsObservable()
                .DistinctUntilChanged();
            // Status
            this._statusSubject = new BehaviorSubject<PlaybackStatus>(PlaybackStatus.None).DisposeWith(this._playerScopeDisposables);
            this.WhenStatusChanged = this._statusSubject
                .AsObservable()
                .DistinctUntilChanged();
            // Volume
            this._volumeSubject = new BehaviorSubject<float>(this._volume).DisposeWith(this._playerScopeDisposables);
            this.WhenVolumeChanged = this._volumeSubject.AsObservable().DistinctUntilChanged();

            // position updater
            this._whenPositionChangedSubscriber = Observable
                  .Interval(this._positionUpdatesInterval, RxApp.TaskpoolScheduler) // TODO: learn about thread pools
                  .Select(_ => this.__soundOut?.WaveSource?.GetPosition())
                  .DistinctUntilChanged()
                  .Publish();
            this._whenPositionChangedSubscriber
                .Subscribe(position => this._positionSubject.OnNext(position))
                .DisposeWith(this._playerScopeDisposables);

            // when started/resumed -> start updating position
            this.WhenStatusChanged
                .Where(status => status == PlaybackStatus.Playing)
                .Subscribe(status =>
                    this.__whenPositionChangedSubscription = this._whenPositionChangedSubscriber
                        .Connect()
                        .DisposeWith(this.__playbackScopeDisposables))
                .DisposeWith(this._playerScopeDisposables);

            // stop updating position when getting into a non-seekable status
            this.WhenStatusChanged
                .Where(status => !PlaybackStatusHelper.CanSeekPlaybackStatuses.Contains(status))
                .Subscribe(status => this.__whenPositionChangedSubscription?.Dispose())
                .DisposeWith(this._playerScopeDisposables);

            // stop listening to stopped event when playback ends/is stopped/interrupted
            this.WhenStatusChanged
                .Where(status => PlaybackStatusHelper.StoppedPlaybackStatuses.Contains(status))
                .Subscribe(status => this.__whenSoundOutStoppedSubscription?.Dispose())
                .DisposeWith(this._playerScopeDisposables);

            // when track loaded -> set position to zero
            this.WhenStatusChanged
                .Where(status => status == PlaybackStatus.Loaded)
                .Subscribe(status => this._positionSubject.OnNext(TimeSpan.Zero))
                .DisposeWith(this._playerScopeDisposables);

            // when the playback stops (not paused)
            this.WhenStatusChanged
                .Where(status =>
                    status == PlaybackStatus.None
                    || status == PlaybackStatus.PlayedToEnd
                    || status == PlaybackStatus.ManuallyInterrupted
                    || status == PlaybackStatus.Exploded)
                .Subscribe(status =>
                {
                    // if played completely to end, first set position to total duration (for UI pleasure)
                    // TODO: can the UI specific total length update be moved to the VM as it is no responsibility of the service?
                    if (status == PlaybackStatus.PlayedToEnd)
                        this._positionSubject.OnNext(this.__soundOut?.WaveSource?.GetLength());

                    // then to null in any stop-case
                    this._positionSubject.OnNext(null);

                    // dispose previous playback resources and reset subscriptions
                    this.__playbackScopeDisposables.Clear();
                })
                .DisposeWith(this._playerScopeDisposables);
            // Duration
            this.WhenDurationChanged = this.WhenStatusChanged
                .Where(status =>
                    status == PlaybackStatus.None
                    || status == PlaybackStatus.Loaded
                    || status == PlaybackStatus.PlayedToEnd
                    || status == PlaybackStatus.ManuallyInterrupted
                    || status == PlaybackStatus.Exploded)
                .Select(status => (status == PlaybackStatus.Loaded) ? this.__soundOut?.WaveSource?.GetLength() : null)
                .DistinctUntilChanged();

            // playback stopped subscriber/subscription
            this._whenSoundOutStoppedEventSubscriber = Observable
                 .FromEventPattern<PlaybackStoppedEventArgs>(
                     h => this.__soundOut.Stopped += h,
                     h => this.__soundOut.Stopped -= h)
                 .Take(1) // TODO: First vs Take(1)
                 .Publish();
            // manual inturruption removes the .Stopped handler before calling .Stop()
            this._whenSoundOutStoppedEventSubscriber
                 .Subscribe(e => this._statusSubject.OnNext(e.EventArgs.HasError ? PlaybackStatus.Exploded : PlaybackStatus.PlayedToEnd))
                 .DisposeWith(this._playerScopeDisposables);

            #region CAN's

            this._canLoadSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanLoadPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanLoadChanged = this._canLoadSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanLoadPlaybackStatuses.Contains(status))
                .Subscribe(can => this._canLoadSubject.OnNext(can))
                .DisposeWith(this._playerScopeDisposables);

            // Can - Play
            this._canPlaySubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanPlayPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanPlayChanged = this._canPlaySubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanPlayPlaybackStatuses.Contains(status))
                .Subscribe(can => this._canPlaySubject.OnNext(can))
                .DisposeWith(this._playerScopeDisposables);

            // Can - Pause
            this._canPauseSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanPauseChanged = this._canPauseSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(status))
                .Subscribe(can => this._canPauseSubject.OnNext(can))
                .DisposeWith(this._playerScopeDisposables);

            // Can - Resume
            this._canResumeSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanResumeChanged = this._canResumeSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(status))
                .Subscribe(can => this._canResumeSubject.OnNext(can))
                .DisposeWith(this._playerScopeDisposables);

            // Can - Stop
            this._canStopSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanStopChanged = this._canStopSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(status))
                .Subscribe(can => this._canStopSubject.OnNext(can))
                .DisposeWith(this._playerScopeDisposables);

            // Can - Seek
            this._canSeekSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanSeekPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanSeekChanged = this._canSeekSubject.AsObservable().DistinctUntilChanged();
            this.WhenStatusChanged
                .Subscribe(status => this._canSeekSubject.OnNext((this.__soundOut?.WaveSource?.CanSeek ?? false) && PlaybackStatusHelper.CanSeekPlaybackStatuses.Contains(status)))
                .DisposeWith(this._playerScopeDisposables);

            #endregion

            #region logging

            // TODO: log

            // track location change log
            this.WhenAudioSourceLocationChanged.Subscribe(tl => Debug.WriteLine($"Track\t\t=\t{tl?.ToString() ?? "null"}")).DisposeWith(this._playerScopeDisposables);
            // position change log
            //this.WhenPositionChanged
            //    .Sample(TimeSpan.FromMilliseconds(500))
            //    .Subscribe(pos => Debug.WriteLine($"Position\t=\t{pos?.ToString() ?? "null"}")).DisposeWith(this.__playerScopeDisposables);
            // status change log
            this.WhenStatusChanged.Subscribe(status => Debug.WriteLine($"{this.GetType().Name}.{nameof(PlaybackStatus)}\t\t=\t{Enum.GetName(typeof(PlaybackStatus), status)}")).DisposeWith(this._playerScopeDisposables);
            // duration chage log
            this.WhenDurationChanged.Subscribe(duration => Debug.WriteLine($"Duration\t=\t{duration?.ToString() ?? "null"}")).DisposeWith(this._playerScopeDisposables);
            // volume change log
            this.WhenVolumeChanged.Throttle(TimeSpan.FromMilliseconds(1000)).Subscribe(volume => Debug.WriteLine($"Volume\t\t=\t{volume}")).DisposeWith(this._playerScopeDisposables);

            #endregion
        }

        #endregion

        #region methods

        #region public

        public async Task LoadAsync(Uri audioSourceLocation)
        {
            await this._playbackActionsSemaphore.WaitAsync();

            // TODO: try-catch inside or outside the Task.Run?
            try
            {
                if (this._canLoadSubject.Value)
                {
                    this._statusSubject.OnNext(PlaybackStatus.Loading); // TODO: notify Loading before or after checking for trackLocation validity?

                    await Task.WhenAll(
                        //Task.Delay(TimeSpan.FromSeconds(1)),
                        Task.Run(() =>
                        {
                            // TODO: expose and make selectable internal playback engine
                            if (WasapiOut.IsSupportedOnCurrentPlatform)
                                this.__soundOut = new WasapiOut();
                            else
                                this.__soundOut = new DirectSoundOut(100);

                            this.__soundOut.Initialize(CodecFactory.Instance.GetCodec(audioSourceLocation));
                            this.__soundOut.Volume = this._volume;

                            // register playbackScopeDisposables
                            this.__soundOut.WaveSource.DisposeWith(this.__playbackScopeDisposables);
                            this.__soundOut.DisposeWith(this.__playbackScopeDisposables);

                            // start listening to ISoundOut.Stopped
                            this.__whenSoundOutStoppedSubscription = this._whenSoundOutStoppedEventSubscriber.Connect();
                        }));

                    this._statusSubject.OnNext(PlaybackStatus.Loaded);
                }
            }
            catch (Exception ex)
            {
                // TODO: move to logger
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(LoadAsync)}: {ex.Message}");

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
                // TODO: this might be moved/duped at loading start
                this.__playbackScopeDisposables.Clear();

                this._statusSubject.OnNext(PlaybackStatus.Exploded);

                throw ex;
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public async Task PlayAsync()
        {
            await this._playbackActionsSemaphore.WaitAsync();

            try
            {
                if (this._canPlaySubject.Value)
                {
                    await Task.Run(() => this.__soundOut.Play());
                    SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Playing);
                    this._statusSubject.OnNext(PlaybackStatus.Playing);
                }
            }
            catch (Exception ex)
            {
                // TODO: move to logger
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(PlayAsync)}: {ex.Message}");

                this._statusSubject.OnNext(PlaybackStatus.Exploded);
                //throw ex;
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public async Task PauseAsync()
        {
            await this._playbackActionsSemaphore.WaitAsync();

            try
            {
                if (this._canPauseSubject.Value)
                {
                    await Task.Run(() => this.__soundOut.Pause());
                    SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Paused);
                    this._statusSubject.OnNext(PlaybackStatus.Paused);
                }
            }
            catch (Exception ex)
            {
                // TODO: move to logger
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(PauseAsync)}: {ex.Message}");
                this._statusSubject.OnNext(PlaybackStatus.Exploded);
                //throw;
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public async Task ResumeAsync()
        {
            await this._playbackActionsSemaphore.WaitAsync();

            try
            {
                if (this._canResumeSubject.Value)
                {
                    await Task.Run(() => this.__soundOut.Resume());
                    SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Playing);
                    this._statusSubject.OnNext(PlaybackStatus.Playing);
                }
            }
            catch (Exception ex)
            {
                // TODO: move to logger
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(ResumeAsync)}: {ex.Message}");
                this._statusSubject.OnNext(PlaybackStatus.Exploded);
                //throw;
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public async Task StopAsync()
        {
            await this._playbackActionsSemaphore.WaitAsync();

            try
            {
                if (this._canStopSubject.Value)
                {
                    // stop listening to the Stopped event
                    this.__whenSoundOutStoppedSubscription.Dispose();

                    await Task.Run(() =>
                    {
                        // TODO: consider adding the WaitForStopped( timeout )
                        // TODO: compare WaitForStopped which uses a WaitHandle https://github.com/filoe/cscore/blob/29410b12ae35321c4556b072c0711a8f289c0544/CSCore/Extensions.cs#L410 vs SpinWait.SpinUntil
                        this.__soundOut.Stop();
                        this.__soundOut.WaitForStopped();
                    });
                    SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Stopped);
                    this._statusSubject.OnNext(PlaybackStatus.ManuallyInterrupted);
                }
            }
            catch (Exception ex)
            {
                // TODO: log
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(StopAsync)}: {ex.Message}");
                this._statusSubject.OnNext(PlaybackStatus.Exploded);
                //throw;
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public async Task SeekToAsync(TimeSpan position)
        {
            await this._playbackActionsSemaphore.WaitAsync();

            try
            {
                if (this._canSeekSubject.Value)
                {
                    if (position < TimeSpan.Zero || position > this.__soundOut.WaveSource.GetLength())
                        throw new ArgumentOutOfRangeException(nameof(position), position, $"{nameof(position)} out of {nameof(IWaveSource)} range."); // TODO: localize

                    await Task.Run(() => this.__soundOut.WaveSource.SetPosition(position));
                    this._positionSubject.OnNext(this.__soundOut?.WaveSource?.GetPosition());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(SetVolume)}: {ex.Message}");
                // it might happen that the ISoundOut is "not initialized yet" -> swallow it
                // TODO: find out why CSCore source code
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        // TODO: ensure concurrency management is good
        private object _volumeLock = new object();
        private float _volume = 0.5f;
        public void SetVolume(float volume)
        {
            try
            {
                lock (this._volumeLock)
                {
                    if (this.__soundOut != null)
                        this.__soundOut.Volume = volume;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(this.SetVolume)}: {ex.Message}");
                // swallow (might happen that the ISoundOut is "not initialized yet"
                // TODO: find out why CSCore source code

                // if not swallow ->
                // this.__volumeSubject.OnError(ex);
            }
            finally
            {
                this._volume = volume;
                this._volumeSubject.OnNext(this._volume);
            }
        }

        #endregion

        #endregion

        #region observable events

        private readonly BehaviorSubject<Uri> _audioSourceLocationSubject;
        public IObservable<Uri> WhenAudioSourceLocationChanged { get; } // TODO: investigate whether .AsObservable().DistinctUntilChanged() creates a new observable every time someone subscribes

        private readonly IConnectableObservable<TimeSpan?> _whenPositionChangedSubscriber;
        private IDisposable __whenPositionChangedSubscription;
        private ISubject<TimeSpan?> _positionSubject;
        public IObservable<TimeSpan?> WhenPositionChanged { get; }

        public IObservable<TimeSpan?> WhenDurationChanged { get; }

        private IConnectableObservable<EventPattern<PlaybackStoppedEventArgs>> _whenSoundOutStoppedEventSubscriber;
        private IDisposable __whenSoundOutStoppedSubscription;
        private readonly BehaviorSubject<PlaybackStatus> _statusSubject;
        public IObservable<PlaybackStatus> WhenStatusChanged { get; }

        private readonly BehaviorSubject<bool> _canLoadSubject;
        public IObservable<bool> WhenCanLoadChanged { get; }

        private readonly BehaviorSubject<bool> _canPlaySubject;
        public IObservable<bool> WhenCanPlayChanged { get; }

        private readonly BehaviorSubject<bool> _canPauseSubject;
        public IObservable<bool> WhenCanPauseChanged { get; }

        private readonly BehaviorSubject<bool> _canResumeSubject;
        public IObservable<bool> WhenCanResumeChanged { get; }

        private readonly BehaviorSubject<bool> _canStopSubject;
        public IObservable<bool> WhenCanStopChanged { get; }

        private readonly BehaviorSubject<bool> _canSeekSubject;
        public IObservable<bool> WhenCanSeekChanged { get; }

        private readonly BehaviorSubject<float> _volumeSubject;
        public IObservable<float> WhenVolumeChanged { get; }

        #endregion

        #region IDisposable

        // disposables prefixed by _ (single underscore) live for the entire lifetime of the object instance
        private CompositeDisposable _playerScopeDisposables;
        private object _playerScopeDisposingLock = new object();
        // disposables prefixed by __ (double underscore) are transient and can be disposed and recreated multiple times
        private CompositeDisposable __playbackScopeDisposables;

        // TODO: review implementation, also consider if there's some Interlocked way to do it
        public void Dispose()
        {
            try
            {
                lock (this._playerScopeDisposingLock)
                {
                    if (this._playerScopeDisposables != null && !this._playerScopeDisposables.IsDisposed)
                    {
                        // no need to ISoundOut.Stop() because the CompositeDisposable disposes the SoundOut which internally calls .Stop()
                        this._playerScopeDisposables?.Dispose();
                        this._playerScopeDisposables = null;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: log
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(Dispose)}: {ex.Message}");
                throw ex;
            }
        }

        #endregion
    }
}