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
    // TODO: annotate class behavior
    // TODO: investigate IWaveSource.AppendSource
    // TODO: investigate IWaveSource.GetLength: how does it calculates duration? How accurate is it guaranteed to be?
    public class CSCorePlayer : IObservableAudioPlayer, IDisposable // TODO: learn how to handle IDisposable from outside and in general how to handle interfaces which implementations may or may not be IDisposable
    {
        #region constants & fields

        private static readonly TimeSpan PositionUpdatesInterval = TimeSpan.FromMilliseconds(333); // TODO: benchmark update frequency
        // SemaphoreSlim                https://docs.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim?view=netframework-4.7
        // Semaphore vs SemaphoreSlim   https://docs.microsoft.com/en-us/dotnet/standard/threading/semaphore-and-semaphoreslim
        private readonly SemaphoreSlim __playbackActionsSemaphore = new SemaphoreSlim(1, 1);

        private ISoundOut _soundOut;
        private bool? _isManuallyStopping = null; // Note: used to (and ONLY to) mark the stop event to be handled as manually stopped, set on manual stopping, unset on new playback setup

        //private IObservable<ISoundOut> __whenSundOutChanged;
        //private IObservable<IWaveSource> __whenWaveSourceChanged;

        #endregion

        #region ctor

        public CSCorePlayer()
        {
            this.__playerDisposables = new CompositeDisposable();
            this._playbackDisposables = new CompositeDisposable().DisposeWith(this.__playerDisposables);

            // SoundOut & SoundOut.WaveSource
            //this.__whenSundOutChanged = this.WhenAnyValue(_ => _._soundOut).DistinctUntilChanged();
            //this.__whenWaveSourceChanged = this.WhenAnyValue(_ => _._soundOut.WaveSource).DistinctUntilChanged();

            // Track Location
            this.__trackLocationSubject = new BehaviorSubject<Uri>(null).DisposeWith(this.__playerDisposables);
            this.WhenTrackLocationChanged = this.__trackLocationSubject
                .AsObservable()
                .DistinctUntilChanged()
                // logs to the debug console Track Location changes
                .Do(tl => Debug.WriteLine($"Track.Location\t\t=\t{tl?.ToString() ?? "null"}"));

            // Playback Position
            this.__positionSubject = new BehaviorSubject<TimeSpan?>(null);
            this.WhenPositionChanged = this.__positionSubject
                .AsObservable()
                .DistinctUntilChanged()
                // logs to the debug console position changes
                .Do(pos => Debug.WriteLine($"Playback.Position\t=\t{pos?.ToString() ?? "null"}"));
            this.__whenPositionChangedConnectable = Observable
                  .Interval(CSCorePlayer.PositionUpdatesInterval, RxApp.TaskpoolScheduler) // TODO: learn about thread pools
                  .Select(_ => this._soundOut?.WaveSource?.GetPosition())
                  .DistinctUntilChanged()
                  .Publish();
            this.__whenPositionChangedConnectable.Subscribe(this.__positionSubject.OnNext).DisposeWith(this.__playerDisposables);

            // Playback Status
            this.__playbackStatusSubject = new BehaviorSubject<PlaybackStatus>(PlaybackStatus.None).DisposeWith(this.__playerDisposables);
            this.WhenPlaybackStatusChanged = this.__playbackStatusSubject
                .AsObservable()
                .DistinctUntilChanged();
            // Playback Status - Subscriptions
            // logs to the debug console status changes
            this.WhenPlaybackStatusChanged
                .Subscribe(status => Debug.WriteLine($"Playback.Status\t\t=\t{Enum.GetName(typeof(PlaybackStatus), status)}"))
                .DisposeWith(this.__playerDisposables);
            // Playback Status - Subscriptions
            // when track loaded -> set position to zero
            this.WhenPlaybackStatusChanged
                .Where(status => status == PlaybackStatus.Loaded)
                .Subscribe(status => this.__positionSubject.OnNext(TimeSpan.Zero))
                .DisposeWith(this.__playerDisposables);
            // when started/resumed -> start updating position
            this.WhenPlaybackStatusChanged
                .Where(status => status == PlaybackStatus.Playing)
                .Subscribe(status =>
                    this._whenPositionChangedSubscription = this.__whenPositionChangedConnectable
                        .Connect()
                        .DisposeWith(this._playbackDisposables))
                .DisposeWith(this.__playerDisposables);
            // when getting into a non-playing status -> stop updating position
            this.WhenPlaybackStatusChanged
                .Where(status => status != PlaybackStatus.Playing)
                .Subscribe(status => this._whenPositionChangedSubscription?.Dispose())
                .DisposeWith(this.__playerDisposables);
            // when the playback stops (not paused) for any reason -> set position to null
            this.WhenPlaybackStatusChanged
                .Where(status =>
                    status == PlaybackStatus.Ended
                    || status == PlaybackStatus.Interrupted
                    || status == PlaybackStatus.Exploded
                    || status == PlaybackStatus.None)
                .Subscribe(status => this.__positionSubject.OnNext(null))
                .DisposeWith(this.__playerDisposables);

            // Playback Duration
            this.WhenDurationChanged = this.WhenPlaybackStatusChanged
                .Where(status =>
                    status == PlaybackStatus.Loaded
                    || status == PlaybackStatus.Ended
                    || status == PlaybackStatus.Interrupted
                    || status == PlaybackStatus.Exploded)
                .Select(status => (status == PlaybackStatus.Loaded) ? this._soundOut?.WaveSource?.GetLength() : null)
                .DistinctUntilChanged();
            this.WhenDurationChanged
                .Subscribe(duration => Debug.WriteLine($"Playback.Duration\t=\t{duration?.ToString() ?? "null"}"));

            #region CAN's

            // Can - PlayNew
            this.__canPlayNewSubject = new BehaviorSubject<bool>(CSCorePlayer.CanPlayNewPlaybackStatuses.Contains(this.__playbackStatusSubject.Value));
            this.WhenCanPlayNewChanged = this.__canPlayNewSubject.AsObservable().DistinctUntilChanged();
            this.WhenPlaybackStatusChanged
                .Select(status => CSCorePlayer.CanPlayNewPlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canPlayNewSubject.OnNext(can))
                .DisposeWith(this.__playerDisposables);

            // Can - Pause
            this.__canPauseSubject = new BehaviorSubject<bool>(false);
            this.WhenCanPausehanged = this.__canPauseSubject.AsObservable().DistinctUntilChanged();
            this.WhenPlaybackStatusChanged
                .Select(status => CSCorePlayer.CanPausePlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canPauseSubject.OnNext(can))
                .DisposeWith(this.__playerDisposables);

            // Can - Resume
            this.__canResumeSubject = new BehaviorSubject<bool>(false);
            this.WhenCanResumeChanged = this.__canResumeSubject.AsObservable().DistinctUntilChanged();
            this.WhenPlaybackStatusChanged
                .Select(status => CSCorePlayer.CanResumePlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canResumeSubject.OnNext(can))
                .DisposeWith(this.__playerDisposables);

            // Can - Stop
            this.__canStopSubject = new BehaviorSubject<bool>(false);
            this.WhenCanStophanged = this.__canStopSubject.AsObservable().DistinctUntilChanged();
            this.WhenPlaybackStatusChanged
                .Select(status => CSCorePlayer.CanStopPlaybackStatuses.Contains(status))
                .Subscribe(can => this.__canStopSubject.OnNext(can))
                .DisposeWith(this.__playerDisposables);

            // Can - Seek
            //this._canSeekSubject = new BehaviorSubject<bool>(false);
            //this.WhenCanSeekChanged = this._canSeekSubject.AsObservable().DistinctUntilChanged();
            //Observable
            //    .CombineLatest(
            //        this.WhenPlaybackStatusChanged,
            //        this._whenWaveSourceChanged, // TODO: if this is null, the exception is raised in MainWindow ctor, ask on SO how to localize the exception
            //        (status, waveSource) =>
            //            waveSource != null
            //            && waveSource.CanSeek
            //            && CSCorePlayer.CanSeekPlaybackStatuses.Contains(status))
            //    .Subscribe(can => this._canSeekSubject.OnNext(can))
            //    .DisposeWith(this._disposables);

            #endregion
        }

        private IConnectableObservable<TimeSpan?> __whenPositionChangedConnectable;
        private IDisposable _whenPositionChangedSubscription;

        #endregion

        #region properties

        private readonly IReadOnlyList<string> _supportedExtensions = CSCore.Codecs.CodecFactory.Instance.GetSupportedFileExtensions();
        public IReadOnlyList<string> SupportedExtensions => this._supportedExtensions;

        private float _volumeCache = 0.5f; // TODO: review name
        public float Volume
        {
            get { return this._soundOut?.Volume ?? this._volumeCache; }
            set { this._soundOut.Volume = (this._volumeCache = value); }
        }

        #endregion

        #region methods

        #region public

        public async Task PlayNewAsync(Uri trackLocation)
        {
            try
            {
                await this.StopAsync();
                await this.__playbackActionsSemaphore.WaitAsync();

                this._playbackDisposables.Clear();

                this.__playbackStatusSubject.OnNext(PlaybackStatus.Loading); // TODO: notify Loading before or after checking for trackLocation validity?
                this.InitializeTrack(trackLocation);
                this.__playbackStatusSubject.OnNext(PlaybackStatus.Loaded);

                // start playback and update status
                this._soundOut.Play();
                SpinWait.SpinUntil(() => this._soundOut?.PlaybackState == PlaybackState.Playing);
                this.__playbackStatusSubject.OnNext(PlaybackStatus.Playing);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case NotSupportedException nse:
                        break;
                    case ArgumentNullException ane:
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

        public Task ResumeAsync()
        {
            if (this._soundOut != null && this._soundOut.PlaybackState == PlaybackState.Paused)
            {
                this._soundOut?.Resume();

                SpinWait.SpinUntil(() => this._soundOut?.PlaybackState == PlaybackState.Playing);
                this.__playbackStatusSubject.OnNext(PlaybackStatus.Playing);
            }

            return Task.CompletedTask; // TODO: ensure this is a best practice and not a wrong use of ConfigureAwait(false), how does it relate to Task.CompletedTask?
        }

        public async Task PauseAsync()
        {
            await this.__playbackActionsSemaphore.WaitAsync();

            if (this._soundOut != null && this._soundOut.PlaybackState == PlaybackState.Playing)
            {
                this._soundOut?.Pause();

                SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Paused);
                this.__playbackStatusSubject.OnNext(PlaybackStatus.Paused);
            }

            this.__playbackActionsSemaphore.Release();
        }

        public async Task StopAsync()
        {
            await this.__playbackActionsSemaphore.WaitAsync();

            try
            {
                if (this._soundOut != null)
                {
                    if (this._soundOut.PlaybackState == PlaybackState.Playing
                        || this._soundOut.PlaybackState == PlaybackState.Paused)
                    {
                        this._isManuallyStopping = true;

                        this._soundOut?.Stop();
                        this._soundOut?.WaitForStopped(); // TODO: consider adding the WaitForStopped( timeout )
                                                          // TODO: compare WaitForStopped which uses a WaitHandle https://github.com/filoe/cscore/blob/29410b12ae35321c4556b072c0711a8f289c0544/CSCore/Extensions.cs#L410 vs SpinWait.SpinUntil
                        SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Stopped);
                    }
                }
            }
            catch (ArgumentNullException)
            {
                this.__playbackStatusSubject.OnNext(PlaybackStatus.Exploded);
                //throw;
            }
            finally
            {
                this.__playbackActionsSemaphore.Release();
            }
        }

        public Task SeekTo(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private

        private ISubject<TimeSpan?> __positionSubject;
        private ISoundOut GetNewSoundOut()
        {
            ISoundOut soundOut;
            if (WasapiOut.IsSupportedOnCurrentPlatform)
                soundOut = new WasapiOut();
            else
                soundOut = new DirectSoundOut(100);

            soundOut.DisposeWith(this._playbackDisposables);

            return soundOut;
        }

        private void DisposeSoundOutAndWaveSource()
        {
            if (this._soundOut != null)
            {
                if (this._soundOut.WaveSource != null
                    && this.__playerDisposables.Contains(this._soundOut.WaveSource))
                {
                    this.__playerDisposables.Remove(this._soundOut.WaveSource); // TODO: can this be called without checking the contains?
                }

                if (this.__playerDisposables.Contains(this._soundOut)) // TODO: what happens if not contained
                                                                       // TODO: what happens if already disposed
                {
                    this.__playerDisposables.Remove(this._soundOut); // TODO: what happens if already disposed
                                                                     // TODO: what does the description mean by "removed and disposes the FIRST occurence" :O
                    this._soundOut = null;
                }
            }
        }

        private void DisposeAndRecreateSoundOut(ref ISoundOut soundOut)
        {
            this.DisposeSoundOutAndWaveSource();
            soundOut = this.GetNewSoundOut();
        }

        private void InitializeTrack(Uri trackLocation)
        {
            if (WasapiOut.IsSupportedOnCurrentPlatform)
                this._soundOut = new WasapiOut();
            else
                this._soundOut = new DirectSoundOut(100);

            this._soundOut.DisposeWith(this._playbackDisposables);

            var waveSource = CodecFactory.Instance.GetCodec(trackLocation);
            waveSource.DisposeWith(this._playbackDisposables);
            this._soundOut.Initialize(waveSource);
            this._soundOut.WaveSource.DisposeWith(this._playbackDisposables);

            Observable
                .FromEventPattern<PlaybackStoppedEventArgs>(
                    h => this._soundOut.Stopped += h,
                    h => this._soundOut.Stopped -= h)
                .Take(1) // TODO: FirstAsync vs Take(1)
                .Subscribe(e =>
                {
                    PlaybackStatus newStatus;

                    if (e.EventArgs.HasError)
                    {
                        newStatus = PlaybackStatus.Exploded;
                    }
                    else
                    {
                        newStatus = this._isManuallyStopping.HasValue && this._isManuallyStopping.Value ? PlaybackStatus.Interrupted : PlaybackStatus.Ended;
                    }

                    this.__playbackStatusSubject.OnNext(newStatus);
                })
                .DisposeWith(this._playbackDisposables);

            this._isManuallyStopping = null;
        }

        #endregion

        #endregion

        #region observable events

        private readonly BehaviorSubject<Uri> __trackLocationSubject;
        public IObservable<Uri> WhenTrackLocationChanged { get; } // TODO: investigate whether .AsObservable().DistinctUntilChanged() creates a new observable every time someone subscribes

        public IObservable<TimeSpan?> WhenPositionChanged { get; }
        public IObservable<TimeSpan?> WhenDurationChanged { get; }

        private readonly BehaviorSubject<PlaybackStatus> __playbackStatusSubject;
        public IObservable<PlaybackStatus> WhenPlaybackStatusChanged { get; }

        private static readonly PlaybackStatus[] CanPlayNewPlaybackStatuses =
            {
                PlaybackStatus.None,
                PlaybackStatus.Playing,
                PlaybackStatus.Paused,
                PlaybackStatus.Ended,
                PlaybackStatus.Interrupted,
                PlaybackStatus.Exploded
            };
        private readonly BehaviorSubject<bool> __canPlayNewSubject;
        public IObservable<bool> WhenCanPlayNewChanged { get; }

        private static readonly PlaybackStatus[] CanPausePlaybackStatuses = { PlaybackStatus.Playing };
        private readonly BehaviorSubject<bool> __canPauseSubject;
        public IObservable<bool> WhenCanPausehanged { get; }

        private static readonly PlaybackStatus[] CanResumePlaybackStatuses = { PlaybackStatus.Paused };
        private readonly BehaviorSubject<bool> __canResumeSubject;
        public IObservable<bool> WhenCanResumeChanged { get; }

        private static readonly PlaybackStatus[] CanStopPlaybackStatuses = { PlaybackStatus.Playing, PlaybackStatus.Paused };
        private readonly BehaviorSubject<bool> __canStopSubject;
        public IObservable<bool> WhenCanStophanged { get; } //  => this._canStopSubject.AsObservable();

        private static readonly PlaybackStatus[] CanSeekPlaybackStatuses = { PlaybackStatus.Playing, PlaybackStatus.Paused };
        private readonly BehaviorSubject<bool> __canSeekSubject;
        public IObservable<bool> WhenCanSeekChanged { get; }

        #endregion

        #region IDisposable

        private CompositeDisposable __playerDisposables; // TODO: consider SerialDisposable to handle SoundOut creations/destructions
        private CompositeDisposable _playbackDisposables;
        private object __disposingLock = new object();

        public void Dispose() // TODO: review implementation, also consider if there's some Interlocked way to do it
        {
            lock (this.__disposingLock)
            {
                if (!this.__playerDisposables.IsDisposed)
                {
                    this.__playerDisposables.Dispose();
                    this.__playerDisposables = null;
                }
            }
        }

        #endregion
    }
}