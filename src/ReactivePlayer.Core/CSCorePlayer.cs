using CSCore.Tags.ID3;
using CSCore;
using CSCore.Tags;
using CSCore.SoundOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using ReactiveUI;

namespace ReactivePlayer.Core
{
    public class CSCorePlayer : IObservableAudioPlayer
    {
        #region constants & fields

        private readonly CompositeDisposable _disposables; // = new CompositeDisposable();
        private readonly ISubject<IAudioSource> _whenAudioSourceChangedSubject;
        private Uri _trackLocation;

        private ISoundOut _soundOut;
        //private IAudioSource _audioSource = null;

        #endregion

        #region ctor

        public CSCorePlayer()
        {
            this._disposables = new CompositeDisposable();

            this._soundOut = new DirectSoundOut().DisposeWith(this._disposables);

            this.WhenTrackLocationChanged = (this._whenTrackLocationChangedSubject = new Subject<Uri>().DisposeWith(this._disposables))
                .AsObservable()
                .DistinctUntilChanged();

            this.WhenDurationChanged =
                (this._whenAudioSourceChangedSubject = new Subject<IAudioSource>().DisposeWith(this._disposables))
                .Select(audioSource => audioSource?.GetLength() ?? TimeSpan.Zero)
                .DistinctUntilChanged(d => d.Ticks)
                .StartWith(TimeSpan.Zero);

            // check position on interval reading from this.Position
            this.WhenPositionChanged = Observable
                .Interval(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler) // TODO: benchmark update frequency
                .Select(_ => this.Position)
                .DistinctUntilChanged(p => p.Ticks)
                .StartWith(TimeSpan.Zero);

            this.WhenStatusChanged = (this._whenStatusChangedSubject = new BehaviorSubject<PlaybackStatus>(PlaybackStatus.None).DisposeWith(this._disposables))
                .AsObservable()
                .DistinctUntilChanged();

            Observable
                .FromEventPattern<PlaybackStoppedEventArgs>(
                    h => this._soundOut.Stopped += h,
                    h => this._soundOut.Stopped -= h)
                .Subscribe(eventPattern =>
                {
                    // TODO: check if really needed: the event should be raised when the state has already changed
                    SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Stopped);

                    this._whenStatusChangedSubject.OnNext(
                        eventPattern.EventArgs.Exception == null ?
                        PlaybackStatus.Ended :
                        PlaybackStatus.Errored);
                })
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private readonly IReadOnlyList<string> _supportedExtensions = CSCore.Codecs.CodecFactory.Instance.GetSupportedFileExtensions();
        public IReadOnlyList<string> SupportedExtensions => this._supportedExtensions;

        public TimeSpan Position
        {
            get { return this._soundOut?.WaveSource?.GetLength() ?? TimeSpan.Zero; }
            set { this._soundOut?.WaveSource?.SetPosition(value); }
        }

        private float _volumeCache = 0.5f; // TODO: review name
        public float Volume
        {
            get { return this._soundOut?.Volume ?? this._volumeCache; }
            set { this._soundOut.Volume = (this._volumeCache = value); }
        }

        #endregion

        #region methods

        public async Task PlayAsync(Uri trackLocation)
        {
            await Task.Run(async () =>
            {
                await this.StopAsync();

                #region new track setup

                this._trackLocation = trackLocation ?? throw new ArgumentNullException(nameof(trackLocation)); // TODO: add msg & localize
                // TODO: add Uri location type (local, online, etc...)
                var audioSource = CSCore.Codecs.CodecFactory.Instance.GetCodec(this._trackLocation.LocalPath);
                this._soundOut.Initialize(audioSource); // TODO: check CSCore source code and check if .Initialize can be called multiple times or should be called only once per ISoundOut allocation

                #endregion

                // notify new track only after complete setup
                this._whenTrackLocationChangedSubject.OnNext(this._trackLocation);

                await this.ResumeAsync().ConfigureAwait(false); // TODO: ensure this is the right usage for .ConfigureAwait(false)
            });
        }

        public async Task ResumeAsync()
        {
            await Task.Run(() =>
            {
                this._soundOut?.Play(); // TODO: check what happens if ISoundOut.Play() when there's no WaveSource

                SpinWait.SpinUntil(() => this._soundOut?.PlaybackState == PlaybackState.Playing);

                this._whenStatusChangedSubject.OnNext(PlaybackStatus.Playing);
            }); // TODO: test with ConfigureAwait(false)
        }

        public async Task PauseAsync()
        {
            await Task.Run(() =>
            {
                this._soundOut?.Pause();

                SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Paused);

                this._whenStatusChangedSubject.OnNext(PlaybackStatus.Paused);
            });
        }

        public async Task StopAsync()
        {
            await Task.Run(() =>
            {
                this._soundOut?.Stop();
                this._soundOut?.WaveSource?.Dispose(); // TODO: can it be dispose in a reactive way, e.g. .DisposeWith??

                SpinWait.SpinUntil(() => this._soundOut.PlaybackState == PlaybackState.Stopped);
                this._whenStatusChangedSubject.OnNext(PlaybackStatus.Stoppped);
            });
        }

        #endregion

        #region observable events

        private readonly ISubject<Uri> _whenTrackLocationChangedSubject;
        public IObservable<Uri> WhenTrackLocationChanged { get; }

        public IObservable<TimeSpan> WhenDurationChanged { get; }

        public IObservable<TimeSpan> WhenPositionChanged { get; }

        private readonly ISubject<PlaybackStatus> _whenStatusChangedSubject;
        public IObservable<PlaybackStatus> WhenStatusChanged { get; }

        public IObservable<bool> WhenCanPlayhanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanPausehanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanStophanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanSeekChanged => throw new NotImplementedException();

        #endregion
    }
}