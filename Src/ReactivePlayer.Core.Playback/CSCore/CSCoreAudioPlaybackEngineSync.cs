using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using ReactivePlayer.Core.Library.Tracks;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback.CSCore
{
    // TODO: buffering support, SingleBlockNotificationStream?? Dopamine docet
    // TODO: investigate IWaveSource.AppendSource
    // TODO: investigate IWaveSource.GetLength: how does it calculate duration? How accurate is it guaranteed to be?
    // TODO: add timeout for audio file loading?
    // TODO: consider using Lock (System.Reactive.Core)
    // TODO: log
    // TODO: learn about thread pools, schedulers etc
    // TODO: consider removing subjects from can's and use a select + startswith on statuschanged + replay(1)
    public class CSCoreAudioPlaybackEngineSync : IAudioPlaybackEngineSync, IDisposable
    {
        #region constants & fields

        // prefixed by _ (single underscore) live for the entire lifetime of the object instance
        // prefixed by __ (double underscore) are transient and can be disposed and recreated at every playback session

        // replace with ReaderWriter semaphore to manage position/volume get/set
        private readonly SemaphoreSlim _playbackActionsSemaphore = new SemaphoreSlim(1, 1);
        private readonly TimeSpan _positionUpdatesInterval = TimeSpan.FromMilliseconds(100);

        private ISoundOut _soundOut;

        #endregion

        #region ctor

        static CSCoreAudioPlaybackEngineSync()
        {
            //if (!CodecFactory.Instance.GetSupportedFileExtensions().Contains(".ogg"))
            //{
            //    CodecFactory.Instance.Register("ogg-vorbis", new CodecFactoryEntry(s => new NVorbisSource(s).ToWaveSource(), ".ogg"));
            //}
        }

        public CSCoreAudioPlaybackEngineSync(/*TimeSpan positionUpdatesInterval = default(TimeSpan)*/)
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
            this.WhenStatusChanged = this._statusSubject.DistinctUntilChanged();

            // duration
            this._durationSubject = new BehaviorSubject<TimeSpan?>(this._soundOut?.WaveSource?.GetLength()).DisposeWith(this._playerScopeDisposables);
            this.WhenDurationChanged = this._durationSubject.DistinctUntilChanged();

            // position
            this._positionSubject = new BehaviorSubject<TimeSpan?>(this._soundOut?.WaveSource?.GetPosition()).DisposeWith(this._playerScopeDisposables);
            this.WhenPositionChanged = this._positionSubject.DistinctUntilChanged();

            #region CAN's

            // can load
            this._canLoadSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.LoadablePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanLoadChanged = this._canLoadSubject.DistinctUntilChanged();

            // can play
            this._canPlaySubject = new BehaviorSubject<bool>(PlaybackStatusHelper.PlayablePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanPlayChanged = this._canPlaySubject.DistinctUntilChanged();

            // can pause
            this._canPauseSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.PausablePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanPauseChanged = this._canPauseSubject.DistinctUntilChanged();

            // can resume
            this._canResumeSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.ResumablePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanResumeChanged = this._canResumeSubject.DistinctUntilChanged();

            // can stop
            this._canStopSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.StoppablePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanStopChanged = this._canStopSubject.DistinctUntilChanged();

            // can seek
            this._canSeekSubject = new BehaviorSubject<bool>(PlaybackStatusHelper.SeekablePlaybackStatuses.Contains(this._statusSubject.Value)).DisposeWith(this._playerScopeDisposables);
            this.WhenCanSeekChanged = this._canSeekSubject.DistinctUntilChanged();

            #endregion

            #region logging

            //this.WhenStatusChanged.Subscribe(status => Debug.WriteLine($"{this.GetType().Name}.{nameof(PlaybackStatus)}\t\t=\t{Enum.GetName(typeof(PlaybackStatus), status)}")).DisposeWith(this._playerScopeDisposables);

            #endregion
        }

        #endregion

        #region methods

        #region engine implementation

        private ISoundOut GetNewSoundOut()
        {
            ISoundOut soundOut;

            // TODO: make selectable internal playback engine?
            if (WasapiOut.IsSupportedOnCurrentPlatform)
                soundOut = new WasapiOut();
            else
                soundOut = new DirectSoundOut(100); // TODO: investigate latency role

            return soundOut;
        }

        public void Load(Track track)
        {
            try
            {
                this._playbackActionsSemaphore.Wait();

                if (this._canLoadSubject.Value)
                {
                    this._statusSubject.OnNext(PlaybackStatus.Loading);

                    try
                    {
                        this._soundOut = this.GetNewSoundOut().DisposeWith(this._playerScopeDisposables);

                        //Thread.Sleep(1500);

                        var waveSource = CodecFactory.Instance.GetCodec(track.Location).DisposeWith(this.__playbackScopeDisposables);
                        // TODO: this line throws null exception if track location file does not exist
                        this._soundOut.Initialize(waveSource);

                        this._soundOut.Volume = this._volumeSubject.Value;
                    }
                    catch (Exception ex)
                    {
                        // TODO: improve this error handling
                        this.HandleSoundOutStoppedEvent(ex);
                    }

                    //await Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(t => loadTask);

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

        public void Play()
        {
            try
            {
                this._playbackActionsSemaphore.Wait();

                if (this._canPlaySubject.Value)
                {
                    this._soundOut.Play();
                    SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Playing);

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

        public void Pause()
        {
            try
            {
                this._playbackActionsSemaphore.Wait();

                if (this._canPauseSubject.Value)
                {
                    this._soundOut.Pause();
                    SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Paused);

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

        public void Resume()
        {
            try
            {
                this._playbackActionsSemaphore.Wait();

                if (this._canResumeSubject.Value)
                {
                    this._soundOut.Resume();
                    SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Playing);

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

        public void Stop()
        {
            try
            {
                this._playbackActionsSemaphore.Wait();

                if (this._canStopSubject.Value)
                {
                    this.DetachFromISoundOutStoppedEvent();

                    this._soundOut.Stop();
                    // TODO: compare WaitForStopped which uses a WaitHandle https://github.com/filoe/cscore/blob/29410b12ae35321c4556b072c0711a8f289c0544/CSCore/Extensions.cs#L410 vs SpinWait.SpinUntil
                    // TODO: consider using the timeout overload
                    this._soundOut.WaitForStopped();
                    SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Stopped);

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

        public void SeekTo(TimeSpan position)
        {
            //throw new NotImplementedException();

            try
            {
                this._playbackActionsSemaphore.Wait();

                if (this._canSeekSubject.Value)
                {
                    //if (position < TimeSpan.Zero || position > this._soundOut.WaveSource.GetLength())
                    //    throw new ArgumentOutOfRangeException(nameof(position), position, $"{nameof(position)} out of {nameof(IWaveSource)} range.");

                    // TODO: check what happens if trying to seek out of duration boundaries

                    this._soundOut.WaveSource.SetPosition(position);
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

            this._canLoadSubject.OnNext(PlaybackStatusHelper.LoadablePlaybackStatuses.Contains(playbackStatus));
            this._canPlaySubject.OnNext(PlaybackStatusHelper.PlayablePlaybackStatuses.Contains(playbackStatus));
            this._canPauseSubject.OnNext(PlaybackStatusHelper.PausablePlaybackStatuses.Contains(playbackStatus));
            this._canResumeSubject.OnNext(PlaybackStatusHelper.ResumablePlaybackStatuses.Contains(playbackStatus));
            this._canStopSubject.OnNext(PlaybackStatusHelper.StoppablePlaybackStatuses.Contains(playbackStatus));
            this._canSeekSubject.OnNext(PlaybackStatusHelper.SeekablePlaybackStatuses.Contains(playbackStatus));
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
                .Subscribe(stoppedEvent =>
                {
                    this._playbackActionsSemaphore.Wait();

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
                Debug.WriteLine(Environment.NewLine + $"{exception.GetType().Name} thrown in {this.GetType().Name}.{nameof(Load)}: {exception.Message}");
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

        private IConnectableObservable<TimeSpan?> _positionPoller;
        private IDisposable __when_POSITION_ChangedSubscription;

        private void CreatePositionUpdater()
        {
            this._positionPoller = Observable
                .Interval(this._positionUpdatesInterval, System.Reactive.Concurrency.DispatcherScheduler.Current) // TODO: use taskpool scheduler?
                .Select(_ => this._soundOut?.WaveSource?.GetPosition())
                .DistinctUntilChanged() // TODO: does .DistinctUntilChanged() filter equal value in respect to a subscription which contains a startwith like the one here below?
                .Publish();

            this._positionPoller
                .StartWith(this._soundOut?.WaveSource?.GetPosition()) // startwith here in this block so it gets current position of when subscription is connected
                .Subscribe(position => this._positionSubject.OnNext(position))
                .DisposeWith(this._playerScopeDisposables);
        }

        private void ActivatePositionUpdater()
        {
            if (this.__when_POSITION_ChangedSubscription == null)
            {
                this.__when_POSITION_ChangedSubscription = this._positionPoller.Connect();
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

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                // free managed resources here
                this._playerScopeDisposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}