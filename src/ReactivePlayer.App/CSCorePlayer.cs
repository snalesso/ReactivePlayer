using CSCore;
using CSCore.SoundOut;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.App
{
    // TODO: investigate IWaveSource.AppendSource
    // TODO: investigate IWaveSource.GetLength: how does it calculates duration? How accurate is it guaranteed to be?
    public class CSCorePlayer : IObservableAudioPlayer, IDisposable // TODO: learn how to handle IDisposable from outside and in general how to handle interfaces which implementations may or may not be IDisposable
    {
        #region constants & fields

        private static readonly TimeSpan PositionPollingFrequency = TimeSpan.FromMilliseconds(250); // TODO: benchmark update frequency

        private readonly CompositeDisposable _disposables; // TODO: consider SerialDisposable to handle SoundOut creations/destructions
        // SemaphoreSlim                https://docs.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim?view=netframework-4.7
        // Semaphore vs SemaphoreSlim   https://docs.microsoft.com/en-us/dotnet/standard/threading/semaphore-and-semaphoreslim
        private readonly SemaphoreSlim _playbackActionsSemaphore = new SemaphoreSlim(1, 1);

        private IObservable<ISoundOut> _whenSoundOutChanged;
        private IObservable<EventPattern<PlaybackStoppedEventArgs>> _whenSoundOutStopped;
        private IDisposable _whenSoundOutStoppedSubscription;

        private Uri _trackLocation;
        private ISoundOut _soundOut;
        private bool _isManuallyStopping = false;

        #endregion

        #region ctor

        public CSCorePlayer()
        {
            this._disposables = new CompositeDisposable();
            this._disposables.Add(this._playbackActionsSemaphore);
            this._disposables.Add(this._playbackActionsSemaphore);

            this._whenSoundOutChanged = this.WhenAnyValue(@this => @this._soundOut).DistinctUntilChanged();
            this._trackLocationSubject = new BehaviorSubject<Uri>(this._trackLocation).DisposeWith(this._disposables);

            this.WhenTrackLocationChanged = this._trackLocationSubject.AsObservable().DistinctUntilChanged();

            // check position on interval reading from this.Position
            this.WhenPositionChanged = Observable
                .Interval(CSCorePlayer.PositionPollingFrequency, RxApp.TaskpoolScheduler)
                .Select(_ => this._soundOut?.WaveSource?.GetPosition())
                .TakeWhile(position => position.HasValue)
                .DistinctUntilChanged(p => p)
                .StartWith(null as TimeSpan?);

            // the behavior that holds and publishes the current PlaybackStatus to the outside
            this._playbackStatusSubject = new BehaviorSubject<PlaybackStatus>(PlaybackStatus.NaturallyEnded).DisposeWith(this._disposables); // TODO: use ManuallyStopped? If at startup listeners read
            this.WhenStatusChanged = this._playbackStatusSubject
                .AsObservable()
                .DistinctUntilChanged();

            // Can[X]

            //this.WhenCanResumeChanged = this.WhenStatusChanged
            //    .Select(status => status == PlaybackStatus.Paused)
            //    .DistinctUntilChanged();

            //this.WhenCanPausehanged = this.WhenStatusChanged
            //    .Select(status => status == PlaybackStatus.Playing)
            //    .DistinctUntilChanged();

            //this.WhenCanPausehanged = this.WhenStatusChanged
            //    .Select(status => status == PlaybackStatus.Playing)
            //    .DistinctUntilChanged();

            //this.WhenCanStophanged = this.WhenStatusChanged
            //    .Select(status =>
            //        status == PlaybackStatus.Loading
            //        || status == PlaybackStatus.Playing
            //        || status == PlaybackStatus.Paused)
            //    .DistinctUntilChanged();

            //this.WhenCanSeekChanged = this.WhenAnyValue(@this =>this._soundOut != null);
        }

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
                if (this._soundOut != null)
                {
                    if (this._soundOut.PlaybackState == PlaybackState.Playing
                        || this._soundOut.PlaybackState == PlaybackState.Paused)
                    {
                        // TODO: check if new track is the same as the currently loaded we can just reposition it to the start
                        await this.StopAsync();
                    }

                    await this.DisposeSoundOutAndWaveSource();
                }

                await this._playbackActionsSemaphore.WaitAsync();

                this._playbackStatusSubject.OnNext(PlaybackStatus.Loading); // TODO: think: should notify loading before or after checking for trackLocation validity?

                this._trackLocation = trackLocation ?? throw new ArgumentNullException(nameof(trackLocation)); // TODO: localize
                                                                                                               // TODO: add Uri location type (local, online, etc...)
                var audioSource = CSCore.Codecs.CodecFactory.Instance.GetCodec(this._trackLocation);
                if (this._soundOut == null)
                {
                    this._soundOut = new WasapiOut(true, CSCore.CoreAudioAPI.AudioClientShareMode.Shared, 100);
                    Observable
                           .FromEventPattern<PlaybackStoppedEventArgs>(
                               h => this._soundOut.Stopped += h,
                               h => this._soundOut.Stopped -= h)
                        .Take(1)
                        .Subscribe(eventPattern =>
                        {
                            // TODO: check if really needed: the event should be raised when the state has already changed
                            SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Stopped);

                            PlaybackStatus status = PlaybackStatus.NaturallyEnded;

                            if (eventPattern.EventArgs.HasError == false)
                            {
                                if (this._isManuallyStopping == false)
                                    status = PlaybackStatus.NaturallyEnded;
                                else
                                    status = PlaybackStatus.ManuallyStopped;
                            }
                            else // implosion driven stop
                            {
                                status = PlaybackStatus.Exploded;
                            }

                            this._playbackStatusSubject.OnNext(status);
                        })
                        .DisposeWith(this._disposables);
                }
                this._soundOut.Initialize(audioSource); // TODO: check CSCore source code and check if .Initialize can be called multiple times or should be called only once per ISoundOut allocation

                this._playbackStatusSubject.OnNext(PlaybackStatus.Loaded);
                this._trackLocationSubject.OnNext(this._trackLocation);

                this._soundOut.Play(); // TODO: ensure this is the right usage for .ConfigureAwait(false)
                SpinWait.SpinUntil(() => this._soundOut?.PlaybackState == PlaybackState.Playing);
                this._playbackStatusSubject.OnNext(PlaybackStatus.Playing);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case NotSupportedException nse:
                        break;
                }

                this._playbackStatusSubject.OnNext(PlaybackStatus.Exploded);

                throw ex;
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public Task ResumeAsync()
        {
            if (this._soundOut != null && this._soundOut.PlaybackState == PlaybackState.Paused)
            {
                this._soundOut?.Resume(); // TODO: check what happens if ISoundOut.Play() when there's no WaveSource

                SpinWait.SpinUntil(() => this._soundOut?.PlaybackState == PlaybackState.Playing);
                this._playbackStatusSubject.OnNext(PlaybackStatus.Playing);
            }

            return Task.CompletedTask; // TODO: ensure this is a best practice and not a wrong use of ConfigureAwait(false), how does it relate to Task.CompletedTask?
        }

        public async Task PauseAsync()
        {
            await this._playbackActionsSemaphore.WaitAsync();

            if (this._soundOut != null && this._soundOut.PlaybackState == PlaybackState.Playing)
            {
                this._soundOut?.Pause();

                SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Paused);
                this._playbackStatusSubject.OnNext(PlaybackStatus.Paused);
            }

            this._playbackActionsSemaphore.Release();
        }

        public async Task StopAsync()
        {
            await this._playbackActionsSemaphore.WaitAsync();

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
                                                          // TODO: compare WaitForStopped which uses a WaitHandle (https://github.com/filoe/cscore/blob/29410b12ae35321c4556b072c0711a8f289c0544/CSCore/Extensions.cs#L410) vs SpinWait.SpinUntil

                        this._isManuallyStopping = false;
                    }
                }
            }
            catch (ArgumentNullException)
            {
                this._playbackStatusSubject.OnNext(PlaybackStatus.Exploded);
                throw;
            }
            finally
            {
                this._playbackActionsSemaphore.Release();
            }
        }

        public Task SeekTo(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private          

        private Task DisposeSoundOutAndWaveSource()
        {
            if (this._soundOut != null)
            {
                if (this._whenSoundOutStoppedSubscription != null)
                {
                    this._disposables.Remove(this._whenSoundOutStoppedSubscription);
                    this._whenSoundOutStoppedSubscription = null;
                    this._whenSoundOutStopped = null;
                }

                if (this._soundOut.WaveSource != null
                    && this._disposables.Contains(this._soundOut.WaveSource))
                {
                    this._disposables.Remove(this._soundOut.WaveSource); // TODO: can this be called without checking the contains?
                }

                if (this._disposables.Contains(this._soundOut)) // TODO: what happens if not contained
                                                                // TODO: what happens if already disposed
                {
                    this._disposables.Remove(this._soundOut); // TODO: what happens if already disposed
                                                              // TODO: what does the description mean by "removed and disposes the FIRST occurence" :O
                    this._soundOut = null;
                }
            }

            return Task.CompletedTask;
        }

        private object _disposingLock = new object();
        public void Dispose() // TODO: review implementation, also consider if there's some Interlocked way to do it
        {
            lock (this._disposingLock)
            {
                if (!this._disposables.IsDisposed)
                    this._disposables.Dispose();
            }
        }

        #endregion

        #endregion

        #region observable events

        private readonly BehaviorSubject<Uri> _trackLocationSubject;
        public IObservable<Uri> WhenTrackLocationChanged { get; } // TODO: investigate whether .AsObservable().DistinctUntilChanged() creates a new observable every time someone subscribes

        public IObservable<TimeSpan?> WhenPositionChanged { get; }

        private readonly BehaviorSubject<PlaybackStatus> _playbackStatusSubject;
        public IObservable<PlaybackStatus> WhenStatusChanged { get; }

        public IObservable<bool> WhenCanPlayNewChanged { get; }

        public IObservable<bool> WhenCanResumeChanged { get; }

        public IObservable<bool> WhenCanPausehanged { get; }

        //private readonly ISubject<bool> _canStopSubject;
        public IObservable<bool> WhenCanStophanged { get; } //  => this._canStopSubject.AsObservable();

        public IObservable<bool> WhenCanSeekChanged { get; }

        #endregion
    }
}