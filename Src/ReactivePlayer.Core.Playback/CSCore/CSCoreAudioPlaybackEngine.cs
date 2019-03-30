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
            this.WhenCanLoadChanged = this._canLoadSubject.DistinctUntilChanged().ObserveOnDispatcher();

            // can play
            this._canPlaySubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanPlayPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanPlayChanged = this._canPlaySubject.DistinctUntilChanged().ObserveOnDispatcher();

            // can pause
            this._canPauseSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanPauseChanged = this._canPauseSubject.DistinctUntilChanged().ObserveOnDispatcher();

            // can resume
            this._canResumeSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanResumeChanged = this._canResumeSubject.DistinctUntilChanged()
                .ObserveOnDispatcher();
            //.ObserveOn(System.Reactive.Concurrency.DispatcherScheduler.Current);

            // can stop
            this._canStopSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanStopChanged = this._canStopSubject.DistinctUntilChanged().ObserveOnDispatcher();

            // can seek
            this._canSeekSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.CanSeekPlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanSeekChanged = this._canSeekSubject.DistinctUntilChanged().ObserveOnDispatcher();

            #endregion

            #region logging

            //this.WhenStatusChanged.Subscribe(status => Debug.WriteLine($"{this.GetType().Name}.{nameof(PlaybackStatus)}\t\t=\t{Enum.GetName(typeof(PlaybackStatus), status)}")).DisposeWith(this._playerScopeDisposables);

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

        public async Task SeekToAsync(TimeSpan position)
        {
            //throw new NotImplementedException();

            try
            {
                await this._playbackActionsSemaphore.WaitAsync();

                if (this._canSeekSubject.Value)
                {
                    //if (position < TimeSpan.Zero || position > this._soundOut.WaveSource.GetLength())
                    //    throw new ArgumentOutOfRangeException(nameof(position), position, $"{nameof(position)} out of {nameof(IWaveSource)} range."); // TODO: localize

                    // TODO: check what happens if trying to seek out of duration boundaries

                    await Task.Run(() => this._soundOut.WaveSource.SetPosition(position));
                    //this._positionSubject.OnNext(this._soundOut?.WaveSource?.GetPosition());
                    this.UpdatePosition(false);
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

        #endregion

        private void UpdateCans()
        {
            PlaybackStatus playbackStatus = this._statusSubject.Value;

            this._canLoadSubject.OnNext(PlaybackStatusHelper.CanLoadPlaybackStatuses.Contains(playbackStatus));
            this._canPlaySubject.OnNext(PlaybackStatusHelper.CanPlayPlaybackStatuses.Contains(playbackStatus));
            this._canPauseSubject.OnNext(PlaybackStatusHelper.CanPausePlaybackStatuses.Contains(playbackStatus));
            this._canResumeSubject.OnNext(PlaybackStatusHelper.CanResumePlaybackStatuses.Contains(playbackStatus));
            this._canStopSubject.OnNext(PlaybackStatusHelper.CanStopPlaybackStatuses.Contains(playbackStatus));
            this._canSeekSubject.OnNext(PlaybackStatusHelper.CanSeekPlaybackStatuses.Contains(playbackStatus));
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
                .Interval(this._positionUpdatesInterval, System.Reactive.Concurrency.DispatcherScheduler.Current) // TODO: use taskpool scheduler?
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
        public TimeSpan? Duration => this._durationSubject.Value;
        public IObservable<TimeSpan?> WhenDurationChanged { get; }

        private readonly BehaviorSubject<TimeSpan?> _positionSubject;
        public TimeSpan? Position => this._positionSubject.Value;
        public IObservable<TimeSpan?> WhenPositionChanged { get; }

        private readonly BehaviorSubject<float> _volumeSubject;
        public IObservable<float> WhenVolumeChanged { get; }

        private readonly BehaviorSubject<bool> _canSeekSubject;
        public IObservable<bool> WhenCanSeekChanged { get; }

        private const float DefaultVolume = 0.25F;
        public float Volume
        {
            get
            {
                this._playbackActionsSemaphore.Wait();

                if (this._soundOut != null)
                {
                    this._soundOut.Volume = this._volumeSubject.Value;
                }

                this._playbackActionsSemaphore.Release();

                return this._volumeSubject.Value;
            }
            set
            {
                try
                {
                    this._playbackActionsSemaphore.Wait();

                    if (this._soundOut != null)
                    {
                        this._soundOut.Volume = value;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(this.Volume)}: {ex.Message}");
                }
                finally
                {
                    this._volumeSubject.OnNext(value);
                    this._playbackActionsSemaphore.Release();
                }
            }
        }

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

        private readonly CompositeDisposable _playerScopeDisposables = new CompositeDisposable();
        private readonly CompositeDisposable __playbackScopeDisposables = new CompositeDisposable();

        // TODO: review implementation, also consider if there's some Interlocked way to do it
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this._playerScopeDisposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this.disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}