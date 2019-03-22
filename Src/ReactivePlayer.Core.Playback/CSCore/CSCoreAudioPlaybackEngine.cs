using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ReactivePlayer.Core.Playback.CSCore
{
    // TODO: buffering support, SingleBlockNotificationStream?? Dopamine docet
    // TODO: investigate IWaveSource.AppendSource
    // TODO: investigate IWaveSource.GetLength: how does it calculate duration? How accurate is it guaranteed to be?
    // TODO: add timeout for audio file loading?
    // TODO: consider using AsyncLock (System.Reactive.Core)
    // TODO: learn how to handle IDisposable from outside and in general how to handle interfaces which implementations may or may not be IDisposable
    // TODO: log
    // TODO: learn about thread pools, schedulers etc
    // TODO: consider removing subjects from can's and use a select + startswith on statuschanged + replay(1)
    public class CSCoreAudioPlaybackEngine : IAudioPlaybackEngine
    {
        // TODO: study SubscribeOn VS ObserveOn, .ToProperty(x, x => x.Property, scheduler: ...), RxApp.MainThreadScheduler.Schedule(() => DoAThing())

        #region constants & fields

        // prefixed by _ (single underscore) live for the entire lifetime of the object instance
        // prefixed by __ (double underscore) are transient and can be disposed and recreated at every playback session

        private readonly SemaphoreSlim _playbackActionsSemaphore = new SemaphoreSlim(1, 1);
        private readonly TimeSpan _positionUpdatesInterval = TimeSpan.FromMilliseconds(100);

        private ISoundOut __soundOut;

        #endregion

        #region ctor

        static CSCoreAudioPlaybackEngine()
        {
            //if (!CodecFactory.Instance.GetSupportedFileExtensions().Contains(".ogg"))
            //{
            //    CodecFactory.Instance.Register("ogg-vorbis", new CodecFactoryEntry(s => new NVorbisSource(s).ToWaveSource(), ".ogg"));
            //}
        }

        public CSCoreAudioPlaybackEngine(/*TimeSpan positionUpdatesInterval = default(TimeSpan)*/)
        {
            //this.__playbackScopeDisposables.DisposeWith(this._playerScopeDisposables);
            this._playbackActionsSemaphore.DisposeWith(this._playerScopeDisposables);

            // track
            this._trackSubject = new BehaviorSubject<Track>(null).DisposeWith(this._playerScopeDisposables);
            this.WhenTrackChanged = this._trackSubject.DistinctUntilChanged();

            // volume
            this._volumeSubject = new BehaviorSubject<float>(DefaultVolume).DisposeWith(this._playerScopeDisposables);
            this.WhenVolumeChanged = this._volumeSubject.DistinctUntilChanged();

            // Status
            this._statusSubject = new BehaviorSubject<PlaybackStatus>(PlaybackStatus.None).DisposeWith(this._playerScopeDisposables);
            this.WhenStatusChanged = this._statusSubject
                //.AsObservable()
                .DistinctUntilChanged()
                .ObserveOnDispatcher()
                ;

            // duration
            this._durationSubject = new BehaviorSubject<TimeSpan?>(this.__soundOut?.WaveSource?.GetLength()).DisposeWith(this._playerScopeDisposables);
            this.WhenDurationChanged = this._durationSubject/*.ObserveOnDispatcher()*/.DistinctUntilChanged();

            // position
            this._positionSubject = new BehaviorSubject<TimeSpan?>(this.__soundOut?.WaveSource?.GetPosition()).DisposeWith(this._playerScopeDisposables);
            this.WhenPositionChanged = this._positionSubject/*.ObserveOnDispatcher()*/.DistinctUntilChanged();

            // handle status changes
            this.WhenStatusChanged
                //.Where(status => PlaybackStatusHelper.StoppedPlaybackStatuses.Contains(status))
                .Subscribe(status =>
                {
                    switch (status)
                    {
                        case PlaybackStatus.None:
                            break;
                        case PlaybackStatus.Loading:
                            break;
                        case PlaybackStatus.Loaded:
                            this.UpdateDuration();
                            this.CreatePositionUpdater();
                            break;
                        case PlaybackStatus.Playing:// TODO: check if task pool scheduler is mandatory
                            this.ActivatePositionUpdater();
                            break;
                        case PlaybackStatus.Paused:
                            this.DeactivatePositionUpdater();
                            break;
                        case PlaybackStatus.ManuallyInterrupted:
                        case PlaybackStatus.PlayedToEnd:
                        case PlaybackStatus.Exploded:
                            this.DeactivatePositionUpdater();
                            this.DestroyPositionUpdater();
                            this.ClearPlaybackScopeObjects();
                            // we update duration after destroying everything, so the duration is null
                            this.UpdateDuration();
                            this.Track = null;
                            break;
                    }
                })
                .DisposeWith(this._playerScopeDisposables);

            #region CAN's

            // can load
            this._canLoadSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanLoadPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanLoadChanged = this._canLoadSubject.DistinctUntilChanged();
            this.WhenStatusChanged
                .Subscribe(status =>
                {
                    this._canLoadSubject.OnNext(PlaybackStatusHelper.CanLoadPlaybackStatuses.Contains(status));
                })
                .DisposeWith(this._playerScopeDisposables);

            // can play
            this._canPlaySubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanPlayPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanPlayChanged = this._canPlaySubject.DistinctUntilChanged();
            this.WhenStatusChanged
                .Subscribe(status =>
                {
                    this._canPlaySubject.OnNext(PlaybackStatusHelper.CanPlayPlaybackStatuses.Contains(status));
                })
                .DisposeWith(this._playerScopeDisposables);

            // can pause
            this._canPauseSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanPauseChanged = this._canPauseSubject.DistinctUntilChanged();
            this.WhenStatusChanged
                .Subscribe(status =>
                {
                    this._canPauseSubject.OnNext(PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(status));
                })
                .DisposeWith(this._playerScopeDisposables);

            // can resume
            this._canResumeSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanResumeChanged = this._canResumeSubject.DistinctUntilChanged();
            this.WhenStatusChanged
                .Subscribe(status =>
                {
                    this._canResumeSubject.OnNext(PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(status));
                })
                .DisposeWith(this._playerScopeDisposables);

            // can stop
            this._canStopSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanStopChanged = this._canStopSubject.DistinctUntilChanged();
            this.WhenStatusChanged
                .Subscribe(status =>
                {
                    this._canStopSubject.OnNext(PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(status));
                })
                .DisposeWith(this._playerScopeDisposables);

            #endregion

            #region logging

            this.WhenStatusChanged.Subscribe(status => Debug.WriteLine($"{this.GetType().Name}.{nameof(PlaybackStatus)}\t\t=\t{Enum.GetName(typeof(PlaybackStatus), status)}")).DisposeWith(this._playerScopeDisposables);

            #endregion
        }

        #endregion

        #region methods

        #region engine implementation

        public async Task LoadAsync(Track track)
        {
            await this._playbackActionsSemaphore.WaitAsync();

            try
            {
                this._statusSubject.OnNext(PlaybackStatus.Loading);

                await
                        //Task.WhenAll(
                        //    Task.Delay(TimeSpan.FromSeconds(1)),
                        Task.Run(() =>
                        {
                            // TODO: make selectable internal playback engine?
                            if (WasapiOut.IsSupportedOnCurrentPlatform)
                                this.__soundOut = new WasapiOut();
                            else
                                this.__soundOut = new DirectSoundOut(100);

                            var codec = CodecFactory.Instance.GetCodec(track.Location);
                            this.__soundOut.Initialize(codec);

                            // start listening to ISoundOut.Stopped
                            // we do it here so the this.Stop unloads the this.Load
                            this.AttachToISoundOutStoppedEvent();

                            this.__soundOut.Volume = this._volumeSubject.Value;

                            // register playbackScopeDisposables
                            this.__soundOut.WaveSource.DisposeWith(this.__playbackScopeDisposables);
                            this.__soundOut.DisposeWith(this.__playbackScopeDisposables);
                        })
                //)
                ;

                this._statusSubject.OnNext(PlaybackStatus.Loaded);
                this.Track = track;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(LoadAsync)}: {ex.Message}");

                switch (ex)
                {
                    case NullReferenceException nullReference:
                        break;
                    case NotSupportedException notSupported:
                        break;
                    case ArgumentNullException argumentNull:
                        break;
                    default:
                        break;
                }

                // ensure resources allocated during the failed loading process are released
                // TODO: this might be moved/duped at loading start
                //this.__playbackScopeDisposables.Clear();

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
                await Task.Run(() =>
                {
                    this.__soundOut.Play();
                });

                SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Playing);
                this._statusSubject.OnNext(PlaybackStatus.Playing);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(PlayAsync)}: {ex.Message}");
                this._statusSubject.OnNext(PlaybackStatus.Exploded);
                throw ex;
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
                await Task.Run(() =>
                {
                    this.__soundOut.Pause();
                });

                SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Paused);
                this._statusSubject.OnNext(PlaybackStatus.Paused);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(PauseAsync)}: {ex.Message}");
                this._statusSubject.OnNext(PlaybackStatus.Exploded);
                throw ex;
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
                await Task.Run(() =>
                {
                    this.__soundOut.Resume();
                });

                SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Playing);
                this._statusSubject.OnNext(PlaybackStatus.Playing);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(ResumeAsync)}: {ex.Message}");
                this._statusSubject.OnNext(PlaybackStatus.Exploded);
                throw ex;
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
                    this.DetachFromISoundOutStoppedEvent();

                    await Task.Run(() =>
                    {
                        this.__soundOut.Stop();
                        // TODO: compare WaitForStopped which uses a WaitHandle https://github.com/filoe/cscore/blob/29410b12ae35321c4556b072c0711a8f289c0544/CSCore/Extensions.cs#L410 vs SpinWait.SpinUntil
                        // TODO: consider using the timeout overload
                        this.__soundOut.WaitForStopped();
                    });

                    SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Stopped);
                    this._statusSubject.OnNext(PlaybackStatus.ManuallyInterrupted);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(StopAsync)}: {ex.Message}");
                this._statusSubject.OnNext(PlaybackStatus.Exploded);
                throw ex;
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public Task SeekToAsync(TimeSpan position)
        {
            throw new NotImplementedException();
            //await this._playbackActionsSemaphore.WaitAsync();

            //try
            //{
            //    if (this._canSeekSubject.Value)
            //    {
            //        if (position < TimeSpan.Zero || position > this.__soundOut.WaveSource.GetLength())
            //            throw new ArgumentOutOfRangeException(nameof(position), position, $"{nameof(position)} out of {nameof(IWaveSource)} range."); // TODO: localize

            //        await Task.Run(() => this.__soundOut.WaveSource.SetPosition(position));
            //        this._positionSubject.OnNext(this.__soundOut?.WaveSource?.GetPosition());
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(SetVolume)}: {ex.Message}");
            //    // it might happen that the ISoundOut is "not initialized yet" -> swallow it
            //    // TODO: find out why CSCore source code
            //}
            //finally
            //{
            //    this._playbackActionsSemaphore.Release();
            //}
        }

        #endregion

        private void UpdateDuration()
        {
            this._durationSubject.OnNext(this.__soundOut?.WaveSource?.GetLength());
        }

        #region ISoundOut.Stopped handling

        private void AttachToISoundOutStoppedEvent()
        {
            this.__when_SoundOut_STOPPED_Subscription = Observable
                .FromEventPattern<PlaybackStoppedEventArgs>(
                    h => this.__soundOut.Stopped += h,
                    h => this.__soundOut.Stopped -= h)
                //.ObserveOn(RxApp.TaskpoolScheduler)
                //.Take(1)
                .Subscribe(e =>
                {
                    this.__when_SoundOut_STOPPED_Subscription.Dispose();
                    this.__when_SoundOut_STOPPED_Subscription = null;

                    if (!e.EventArgs.HasError)
                        this._statusSubject.OnNext(PlaybackStatus.PlayedToEnd);
                    else
                        this._statusSubject.OnNext(PlaybackStatus.Exploded);
                });
        }

        private void DetachFromISoundOutStoppedEvent()
        {
            this.__when_SoundOut_STOPPED_Subscription.Dispose();
            this.__when_SoundOut_STOPPED_Subscription = null;
        }

        #endregion

        #region position updater

        private IConnectableObservable<TimeSpan?> _whenPositionChangedSubscriber;
        private IDisposable __when_POSITION_ChangedSubscription;

        private void CreatePositionUpdater()
        {
            this._whenPositionChangedSubscriber = Observable
                .Interval(this._positionUpdatesInterval, System.Reactive.Concurrency.DispatcherScheduler.Current)
                .Select(_ => this.__soundOut?.WaveSource?.GetPosition())
                .DistinctUntilChanged()
                .Publish();

            this._whenPositionChangedSubscriber
                .StartWith(this.__soundOut?.WaveSource?.GetPosition()) // startwith here so it gets current position of when subscription is connected
                .Subscribe(position => this._positionSubject.OnNext(position))
                .DisposeWith(this._playerScopeDisposables);
        }

        private void ActivatePositionUpdater()
        {
            this.__when_POSITION_ChangedSubscription = this._whenPositionChangedSubscriber.Connect();
        }

        private void DeactivatePositionUpdater()
        {
            try
            {
                if (this.__soundOut != null && this.__soundOut.PlaybackState != PlaybackState.Stopped)
                {
                    this._positionSubject.OnNext(this.__soundOut.WaveSource?.GetPosition());
                }
                else
                {
                    this._positionSubject.OnNext(null);
                }
            }
            catch
            {
                // swallow in case it exploded
                Debug.WriteLine($"{nameof(CSCoreAudioPlaybackEngine)}.{nameof(this.DeactivatePositionUpdater)}: exception getting position");
                this._positionSubject.OnNext(null);
            }
            // null checker because if just Loaded it never started polling for position
            this.__when_POSITION_ChangedSubscription?.Dispose();
        }

        private void DestroyPositionUpdater()
        {
            this._whenPositionChangedSubscriber = null;
        }

        #endregion

        private void ClearPlaybackScopeObjects()
        {
            this.__playbackScopeDisposables.Clear();
            this.__soundOut = null;
        }

        #endregion

        #region observable events

        //private IConnectableObservable<EventPattern<PlaybackStoppedEventArgs>> _whenSoundOutStoppedEventSubscriber;
        private IDisposable __when_SoundOut_STOPPED_Subscription;
        private readonly BehaviorSubject<PlaybackStatus> _statusSubject;
        public IObservable<PlaybackStatus> WhenStatusChanged { get; }

        private readonly BehaviorSubject<bool> _canPlaySubject;
        public IObservable<bool> WhenCanPlayChanged { get; }

        private readonly BehaviorSubject<bool> _canLoadSubject;
        public IObservable<bool> WhenCanLoadChanged { get; }

        private readonly BehaviorSubject<bool> _canResumeSubject;
        public IObservable<bool> WhenCanResumeChanged { get; }

        private readonly BehaviorSubject<bool> _canPauseSubject;
        public IObservable<bool> WhenCanPauseChanged { get; }

        private readonly BehaviorSubject<bool> _canStopSubject;
        public IObservable<bool> WhenCanStopChanged { get; }

        private readonly BehaviorSubject<TimeSpan?> _durationSubject;
        public IObservable<TimeSpan?> WhenDurationChanged { get; }

        private readonly BehaviorSubject<TimeSpan?> _positionSubject;
        public IObservable<TimeSpan?> WhenPositionChanged { get; }

        private readonly BehaviorSubject<float> _volumeSubject;
        public IObservable<float> WhenVolumeChanged { get; }

        // TODO: ensure concurrency management is good
        private object _volumeLock = new object();
        private const float DefaultVolume = 0.5F;
        public float Volume
        {
            get
            {
                lock (this._volumeLock)
                {
                    if (this.__soundOut != null)
                    {
                        this.__soundOut.Volume = this._volumeSubject.Value;
                    }
                    return this._volumeSubject.Value;
                }
            }
            set
            {
                // TODO: try-catch inside or outside lock?
                lock (this._volumeLock)
                {
                    try
                    {
                        if (this.__soundOut != null)
                        {
                            this.__soundOut.Volume = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(this.Volume)}: {ex.Message}");
                        // swallow (might happen that the ISoundOut is "not initialized yet"
                        // TODO: find out why CSCore source code

                        // if not swallow ->
                        // this.__volumeSubject.OnError(ex);
                    }
                    finally
                    {
                        this._volumeSubject.OnNext(value);
                    }
                }
            }
        }

        public IObservable<bool> WhenCanSeekChanged => throw new NotImplementedException();

        private readonly BehaviorSubject<Track> _trackSubject;
        public Track Track
        {
            get => this._trackSubject.Value;
            private set => this._trackSubject.OnNext(value);
        }

        public IObservable<Track> WhenTrackChanged { get; }

        public IReadOnlyList<string> SupportedExtensions => CodecFactory.Instance.GetSupportedFileExtensions();

        #endregion

        #region IDisposable

        private object _playerScopeDisposingLock = new object();
        private CompositeDisposable _playerScopeDisposables = new CompositeDisposable();
        private CompositeDisposable __playbackScopeDisposables = new CompositeDisposable();

        // TODO: review implementation, also consider if there's some Interlocked way to do it
        public void Dispose()
        {
            // TODO: try-catch inside or outside lock?
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