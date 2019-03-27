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

        private ISoundOut _soundOut;

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
            this.WhenTrackChanged = this._trackSubject.DistinctUntilChanged().ObserveOnDispatcher();

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
            this._durationSubject = new BehaviorSubject<TimeSpan?>(this._soundOut?.WaveSource?.GetLength()).DisposeWith(this._playerScopeDisposables);
            this.WhenDurationChanged = this._durationSubject/*.ObserveOnDispatcher()*/.DistinctUntilChanged();

            // position
            this._positionSubject = new BehaviorSubject<TimeSpan?>(this._soundOut?.WaveSource?.GetPosition()).DisposeWith(this._playerScopeDisposables);
            this.WhenPositionChanged = this._positionSubject/*.ObserveOnDispatcher()*/.DistinctUntilChanged();

            #region CAN's

            // can load
            this._canLoadSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanLoadPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanLoadChanged = this._canLoadSubject.DistinctUntilChanged()
                .ObserveOnDispatcher();
            //this.WhenStatusChanged
            //    .Subscribe(status =>
            //    {
            //        this._canLoadSubject.OnNext(PlaybackStatusHelper.CanLoadPlaybackStatuses.Contains(status));
            //    })
            //    .DisposeWith(this._playerScopeDisposables);

            // can play
            this._canPlaySubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanPlayPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanPlayChanged = this._canPlaySubject.DistinctUntilChanged()
                .ObserveOnDispatcher();
            //this.WhenStatusChanged
            //    .Subscribe(status =>
            //    {
            //        this._canPlaySubject.OnNext(PlaybackStatusHelper.CanPlayPlaybackStatuses.Contains(status));
            //    })
            //    .DisposeWith(this._playerScopeDisposables);

            // can pause
            this._canPauseSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanPauseChanged = this._canPauseSubject.DistinctUntilChanged()
                .ObserveOnDispatcher();
            //this.WhenStatusChanged
            //    .Subscribe(status =>
            //    {
            //        this._canPauseSubject.OnNext(PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(status));
            //    })
            //    .DisposeWith(this._playerScopeDisposables);

            // can resume
            this._canResumeSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanResumeChanged = this._canResumeSubject.DistinctUntilChanged()
                .ObserveOnDispatcher()
                //.ObserveOn(System.Reactive.Concurrency.DispatcherScheduler.Current)
                ;
            //this.WhenStatusChanged
            //    .Subscribe(status =>
            //    {
            //        this._canResumeSubject.OnNext(PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(status));
            //    })
            //    .DisposeWith(this._playerScopeDisposables);

            // can stop
            this._canStopSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanStopChanged = this._canStopSubject.DistinctUntilChanged()
                .ObserveOnDispatcher();
            //this.WhenStatusChanged
            //    .Subscribe(status =>
            //    {
            //        this._canStopSubject.OnNext(PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(status));
            //    })
            //    .DisposeWith(this._playerScopeDisposables);

            #endregion

            #region logging

            this.WhenStatusChanged.Subscribe(status => Debug.WriteLine($"{this.GetType().Name}.{nameof(PlaybackStatus)}\t\t=\t{Enum.GetName(typeof(PlaybackStatus), status)}")).DisposeWith(this._playerScopeDisposables);

            #endregion
        }

        #endregion

        #region methods

        #region engine implementation

        // TODO: investigate whether sound out generation should be offloaded
        private ISoundOut GetNewSoundOut()
        {
            ISoundOut soundOut = null;

            // TODO: make selectable internal playback engine?
            if (WasapiOut.IsSupportedOnCurrentPlatform)
                soundOut = new WasapiOut();
            else
                soundOut = new DirectSoundOut(100); // TODO: investigate latency role

            return soundOut;
        }

        public async Task LoadAsync(Track track)
        {
            try
            {
                await this._playbackActionsSemaphore.WaitAsync();

                if (this._canLoadSubject.Value)
                {
                    this._statusSubject.OnNext(PlaybackStatus.Loading);

                    await Task.Run(() =>
                    {
                        this._soundOut = this.GetNewSoundOut().DisposeWith(this._playerScopeDisposables);

                        var waveSource = CodecFactory.Instance.GetCodec(track.Location).DisposeWith(this.__playbackScopeDisposables);
                        this._soundOut.Initialize(waveSource);

                        this._soundOut.Volume = this._volumeSubject.Value;
                    });

                    this.AttachToISoundOutStoppedEvent();
                    this.UpdateDuration(false);
                    this.CreatePositionUpdater();
                    this.Track = track;
                    this._statusSubject.OnNext(PlaybackStatus.Loaded);
                    this.UpdateCans();
                }
            }
            catch (Exception ex)
            {
                this.HandleSoundOutStoppedEvent(ex);
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public async Task PlayAsync()
        {
            try
            {
                await this._playbackActionsSemaphore.WaitAsync();

                if (this._canPlaySubject.Value)
                {
                    await Task.Run(() =>
                    {
                        this._soundOut.Play();
                        SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Playing);
                    });

                    this.ActivatePositionUpdater();

                    this._statusSubject.OnNext(PlaybackStatus.Playing);
                    this.UpdateCans();
                }
            }
            catch (Exception ex)
            {
                this.HandleSoundOutStoppedEvent(ex);
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public async Task PauseAsync()
        {
            try
            {
                await this._playbackActionsSemaphore.WaitAsync();

                if (this._canPauseSubject.Value)
                {
                    await Task.Run(() =>
                    {
                        this._soundOut.Pause();
                        SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Paused);
                    });

                    this._statusSubject.OnNext(PlaybackStatus.Paused);
                    this.UpdateCans();
                }
            }
            catch (Exception ex)
            {
                this.HandleSoundOutStoppedEvent(ex);
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public async Task ResumeAsync()
        {
            try
            {
                await this._playbackActionsSemaphore.WaitAsync();

                if (this._canResumeSubject.Value)
                {
                    await Task.Run(() =>
                    {
                        this._soundOut.Resume();
                        SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Playing);
                    });

                    this.ActivatePositionUpdater();

                    this._statusSubject.OnNext(PlaybackStatus.Playing);
                    this.UpdateCans();
                }
            }
            catch (Exception ex)
            {
                this.HandleSoundOutStoppedEvent(ex);
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public async Task StopAsync()
        {
            try
            {
                await this._playbackActionsSemaphore.WaitAsync();

                if (this._canStopSubject.Value)
                {
                    this.DetachFromISoundOutStoppedEvent();

                    await Task.Run(() =>
                    {
                        this._soundOut.Stop();
                        // TODO: compare WaitForStopped which uses a WaitHandle https://github.com/filoe/cscore/blob/29410b12ae35321c4556b072c0711a8f289c0544/CSCore/Extensions.cs#L410 vs SpinWait.SpinUntil
                        // TODO: consider using the timeout overload
                        this._soundOut.WaitForStopped();
                        SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Stopped);
                    });

                    this.DeactivatePositionUpdater();
                    this.UpdatePosition(true);

                    this.UpdateDuration(true);

                    this.__playbackScopeDisposables.Clear();

                    this.Track = null;

                    this._statusSubject.OnNext(PlaybackStatus.ManuallyInterrupted);
                    this.UpdateCans();
                }
            }
            catch (Exception ex)
            {
                this.HandleSoundOutStoppedEvent(ex);
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

        private void UpdateCans()
        {
            PlaybackStatus playbackStatus = this._statusSubject.Value;

            this._canLoadSubject.OnNext(PlaybackStatusHelper.CanLoadPlaybackStatuses.Contains(playbackStatus));
            this._canPlaySubject.OnNext(PlaybackStatusHelper.CanPlayPlaybackStatuses.Contains(playbackStatus));
            this._canPauseSubject.OnNext(PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(playbackStatus));
            this._canResumeSubject.OnNext(PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(playbackStatus));
            this._canStopSubject.OnNext(PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(playbackStatus));
        }

        private void UpdateDuration(bool isEnded)
        {
            TimeSpan? duration = null;

            try
            {
                if (!isEnded)
                    duration = this._soundOut?.WaveSource?.GetLength();
                //else
                //    duration = null;
            }
            catch //(Exception ex)
            {
                duration = null;
            }
            finally
            {
                this._durationSubject.OnNext(duration);
            }
        }

        private void UpdatePosition(bool isEnded)
        {
            TimeSpan? position = null;

            try
            {
                position = this._soundOut?.WaveSource?.GetPosition();
            }
            catch //(Exception ex)
            {
                position = null;
            }
            finally
            {
                this._positionSubject.OnNext(position);
                // if we updated the position to touch the last tick because playback ended
                // if the current position is not already null (which might already be in some cases, i guess ...)
                // we set it to null
                if (isEnded && position != null)
                    this._positionSubject.OnNext(null);
            }
        }

        #region ISoundOut.Stopped handling

        private void AttachToISoundOutStoppedEvent()
        {
            this.__when_SoundOut_STOPPED_Subscription = Observable
                .FromEventPattern<PlaybackStoppedEventArgs>(
                    h => this._soundOut.Stopped += h,
                    h => this._soundOut.Stopped -= h)
                //.ObserveOn(RxApp.TaskpoolScheduler)
                //.Take(1)
                .Subscribe(async stoppedEvent =>
                {
                    await this._playbackActionsSemaphore.WaitAsync();

                    this.HandleSoundOutStoppedEvent(stoppedEvent.EventArgs.Exception);

                    this._playbackActionsSemaphore.Release();
                });
        }

        private void DetachFromISoundOutStoppedEvent()
        {
            if (this.__when_SoundOut_STOPPED_Subscription != null)
            {
                this.__when_SoundOut_STOPPED_Subscription.Dispose();
                this.__when_SoundOut_STOPPED_Subscription = null;
            }
        }

        private void HandleSoundOutStoppedEvent(Exception exception = null)
        {
            if (exception != null)
            {
                Debug.WriteLine(Environment.NewLine + $"{exception.GetType().Name} thrown in {this.GetType().Name}.{nameof(LoadAsync)}: {exception.Message}");
            }

            // 0 - unsubscribe stopped event
            this.DetachFromISoundOutStoppedEvent();

            // 1 - manually update position
            this.DeactivatePositionUpdater();
            this.UpdatePosition(true);

            // 2 - update duration
            this.UpdateDuration(true);

            // 3 - clear playback scope stuff
            this.__playbackScopeDisposables.Clear();

            this.Track = null;
            this._statusSubject.OnNext(exception == null ? PlaybackStatus.PlayedToEnd : PlaybackStatus.Exploded);
            this.UpdateCans();
        }

        #endregion

        #region position updater

        private IConnectableObservable<TimeSpan?> _whenPositionChanged;
        private IDisposable __when_POSITION_ChangedSubscription;

        private void CreatePositionUpdater()
        {
            this._whenPositionChanged = Observable
                .Interval(this._positionUpdatesInterval, System.Reactive.Concurrency.DispatcherScheduler.Current)
                .Select(_ => this._soundOut?.WaveSource?.GetPosition())
                .DistinctUntilChanged()
                .Publish();

            this._whenPositionChanged
                .StartWith(this._soundOut?.WaveSource?.GetPosition()) // startwith here in this block so it gets current position of when subscription is connected
                .Subscribe(position => this._positionSubject.OnNext(position))
                .DisposeWith(this._playerScopeDisposables);
        }

        private void ActivatePositionUpdater()
        {
            if (this.__when_POSITION_ChangedSubscription == null)
            {
                this.__when_POSITION_ChangedSubscription = this._whenPositionChanged.Connect();
            }
        }

        private void DeactivatePositionUpdater()
        {
            // if just playback Loaded but never started playing, the subscription was never created
            if (this.__when_POSITION_ChangedSubscription != null)
            {
                this.__when_POSITION_ChangedSubscription.Dispose();
                this.__when_POSITION_ChangedSubscription = null;
            }
        }

        #endregion

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
                    if (this._soundOut != null)
                    {
                        this._soundOut.Volume = this._volumeSubject.Value;
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
                        if (this._soundOut != null)
                        {
                            this._soundOut.Volume = value;
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
        private readonly CompositeDisposable _playerScopeDisposables = new CompositeDisposable();
        private readonly CompositeDisposable __playbackScopeDisposables = new CompositeDisposable();

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
                        //this._playerScopeDisposables = null;
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