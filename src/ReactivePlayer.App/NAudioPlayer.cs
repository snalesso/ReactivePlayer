using NAudio;
using NAudio.Wave;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.App
{
    // TODO: verify disposing coverage: are all implementations disposable? what if an implementation must be disposed but is referenced only through IObservableAudioPlayer?????
    public class NAudioPlayer : IObservableAudioPlayer, IDisposable
    {
        #region constants & fields

        private readonly CompositeDisposable _disposables; // = new CompositeDisposable();
        private readonly IWavePlayer _wavePlayer; // = new DirectSoundOut();
        private readonly ISubject<AudioFileReader> _whenAudioFileReaderChangedSubject;
        //private readonly IObservable<PlaybackStatus> _whenPlaybackTerminated;

        private AudioFileReader _audioFileReader;
        private Uri _trackLocation;

        #endregion

        #region ctors

        public NAudioPlayer()
        {
            this._disposables = new CompositeDisposable();

            this._wavePlayer = new DirectSoundOut().DisposeWith(this._disposables);

            this.WhenTrackLocationChanged = (this._whenTrackLocationChangedSubject = new Subject<Uri>().DisposeWith(this._disposables))
                .AsObservable()
                .DistinctUntilChanged();

            this.WhenDurationChanged =
                (this._whenAudioFileReaderChangedSubject = new Subject<AudioFileReader>().DisposeWith(this._disposables))
                .Select(audioFileReader => audioFileReader?.TotalTime ?? TimeSpan.Zero)
                .DistinctUntilChanged(d => d.Ticks)
                .StartWith(TimeSpan.Zero);

            // check position on interval reading from this.Position
            this.WhenPositionChanged = Observable
                .Interval(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler) // TODO: benchmark update frequency
                .Select(_ => this.Position)
                .DistinctUntilChanged(p => p.Ticks)
                .StartWith(TimeSpan.Zero);

            this.WhenPlaybackStatusChanged = (this._whenStatusChangedSubject = new BehaviorSubject<PlaybackStatus>(PlaybackStatus.Ended).DisposeWith(this._disposables))
                .AsObservable()
                .DistinctUntilChanged();

            Observable
                .FromEventPattern<StoppedEventArgs>(
                    h => this._wavePlayer.PlaybackStopped += h,
                    h => this._wavePlayer.PlaybackStopped -= h)
                .Subscribe(eventPattern =>
                {
                    // TODO: check if really needed: the event should be raised when the state has already changed
                    SpinWait.SpinUntil(() => this._wavePlayer.PlaybackState == PlaybackState.Stopped);

                    this._whenStatusChangedSubject.OnNext(
                        eventPattern.EventArgs.Exception == null ?
                        PlaybackStatus.Ended :
                        PlaybackStatus.Exploded);
                })
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private readonly IReadOnlyList<string> _supportedExtensions = new[] { "mp3", "aac", "flac", "m4a", "wav", "wma" }; // TODO: check whether .ToList() is needed: how is .Count property calculated if the the underlying object for an IReadOnlyList<T> property is covertly an Array (with by-index accessibility but no .Count property), is the .Count() extension method used every time?
        public IReadOnlyList<string> SupportedExtensions => this._supportedExtensions;

        public TimeSpan Position
        {
            get { return this._audioFileReader?.CurrentTime ?? TimeSpan.Zero; }
            set { this._audioFileReader.CurrentTime = value; }
        }

        private float _volumeCache = 0.5f; // TODO: review name
        public float Volume
        {
            get { return this._audioFileReader?.Volume ?? this._volumeCache; }
            set
            {
                this._volumeCache = value;

                if (this._audioFileReader != null)
                    this._audioFileReader.Volume = value;
            }
        }

        #endregion

        #region methods

        public async Task PlayNewAsync(Uri trackLocation)
        {
            await this.StopAsync();

            this._trackLocation = trackLocation ?? throw new ArgumentNullException(nameof(trackLocation));

            this._audioFileReader = new AudioFileReader(this._trackLocation.AbsoluteUri)
            {
                Volume = this._volumeCache
            };

            this._whenTrackLocationChangedSubject.OnNext(this._trackLocation);

            //var oldReader = Interlocked.Exchange(ref this._audioFileReader, newReader); // TODO: is Interlocked.Exchange really needed?

            //oldReader?.Dispose();

            this._wavePlayer.Init(this._audioFileReader);

            await this.ResumeAsync().ConfigureAwait(false); // TODO: ensure this is a best practice and not a wrong use of ConfigureAwait(false), how does it relate to Task.CompletedTask?
        }

        public async Task ResumeAsync()
        {
            // TODO: check can play, a track is loaded, non disposed, etc ...
            this._wavePlayer?.Play();

            SpinWait.SpinUntil(() => this._wavePlayer.PlaybackState == PlaybackState.Playing);

            this._whenStatusChangedSubject.OnNext(PlaybackStatus.Playing);

            await Task.CompletedTask; // TODO: investigate if ConfigureAwait(false) can be used
        }

        public async Task PauseAsync()
        {
            this._wavePlayer.Pause();

            SpinWait.SpinUntil(() => this._wavePlayer.PlaybackState == PlaybackState.Paused);

            this._whenStatusChangedSubject.OnNext(PlaybackStatus.Paused);

            await Task.CompletedTask; // TODO: investigate if ConfigureAwait(false) can be used
        }

        public async Task StopAsync()
        {
            this._wavePlayer?.Stop();

            SpinWait.SpinUntil(() => this._wavePlayer.PlaybackState == PlaybackState.Stopped);

            this._audioFileReader?.Dispose();
            this._audioFileReader = null;
            this._trackLocation = null;
            this._whenStatusChangedSubject.OnNext(PlaybackStatus.Ended);

            await Task.CompletedTask; // TODO: test with ConfigureAwait(false)  
        }

        #endregion

        #region observable events

        private readonly ISubject<Uri> _whenTrackLocationChangedSubject;
        public IObservable<Uri> WhenTrackLocationChanged { get; }

        public IObservable<TimeSpan> WhenDurationChanged { get; }

        public IObservable<TimeSpan> WhenPositionChanged { get; }

        private readonly ISubject<PlaybackStatus> _whenStatusChangedSubject;
        public IObservable<PlaybackStatus> WhenPlaybackStatusChanged { get; }

        public IObservable<bool> WhenCanResumeChanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanPausehanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanStophanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanSeekChanged => throw new NotImplementedException();

        IObservable<TimeSpan?> IObservableAudioPlayer.WhenPositionChanged => throw new NotImplementedException();

        public IObservable<bool> WhenSomethingGoesWrong => throw new NotImplementedException();

        public IObservable<bool> WhenCanPlayNewChanged => throw new NotImplementedException();

        IObservable<TimeSpan?> IObservableAudioPlayer.WhenDurationChanged => throw new NotImplementedException();

        public IObservable<float> WhenVolumeChanged => throw new NotImplementedException();

        #endregion

        #region IDisposable

        public void Dispose()
        {
            try
            {
                if (!this._disposables.IsDisposed)
                    this._disposables.Dispose();

                this._wavePlayer?.Stop();

                this._audioFileReader?.Dispose();
                this._audioFileReader = null;

                this._wavePlayer?.Dispose();
            }

            // Weird exception
            catch (MmException)
            {
            }
        }

        public Task SeekTo(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public void SetVolume(float volume)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}