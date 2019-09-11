using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaybackControlsViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        //private readonly IPlaybackService _playbackService;
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        //private readonly PlaybackQueue _playbackQueue;
        //private readonly PlaybackHistory _playbackHistory;
        //private readonly IReadLibraryService _readLibraryService;

        #endregion

        #region constructors

        public PlaybackControlsViewModel(
            //IPlaybackService playbackService,
            IAudioPlaybackEngine audioPlaybackEngine,
            //PlaybackQueue playbackQueue,
            //PlaybackHistory playbackHistory,
            //IReadLibraryService readLibraryService
            PlaybackTimelineViewModel playbackTimelineViewModel
            )
        {
            // TODO: log
            //this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService));
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            //this._playbackQueue = playbackQueue ?? throw new ArgumentNullException(nameof(playbackQueue));
            //this._playbackHistory = playbackHistory ?? throw new ArgumentNullException(nameof(playbackHistory));
            //this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this.PlaybackTimelineViewModel = playbackTimelineViewModel ?? throw new ArgumentNullException(nameof(playbackTimelineViewModel));

            this._volume_OAPH = this._audioPlaybackEngine.WhenVolumeChanged
                .ToProperty(this, nameof(this.Volume))
                .DisposeWith(this._disposables);
            this._hasLoadedTrack_OAPH = this._audioPlaybackEngine.WhenTrackChanged
                .Select(x => x == null)
                .ToProperty(this, nameof(this.HasLoadedTrack))
                .DisposeWith(this._disposables);
            this._canPause_OAPH = this._audioPlaybackEngine.WhenCanPauseChanged
                .ToProperty(this, nameof(this.CanPause))
                .DisposeWith(this._disposables);
            this._canResume_OAPH = this._audioPlaybackEngine.WhenCanResumeChanged
                .ToProperty(this, nameof(this.CanResume))
                .DisposeWith(this._disposables);

            this.PlayAll = ReactiveCommand.Create(
                () => throw new NotImplementedException(),
                Observable.Return(false));
            this.PlayAll.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.PlayAll.DisposeWith(this._disposables);

            this.Pause = ReactiveCommand.CreateFromTask(
                () => this._audioPlaybackEngine.PauseAsync(),
                this._audioPlaybackEngine.WhenCanPauseChanged);
            this.Pause.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.Pause.DisposeWith(this._disposables);

            this.Resume = ReactiveCommand.CreateFromTask(
                () => this._audioPlaybackEngine.ResumeAsync(),
                this._audioPlaybackEngine.WhenCanResumeChanged);
            this.Resume.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.Resume.DisposeWith(this._disposables);

            this.Stop = ReactiveCommand.CreateFromTask(
                () => this._audioPlaybackEngine.StopAsync(),
                this._audioPlaybackEngine.WhenCanStopChanged);
            this.Stop.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.Stop.DisposeWith(this._disposables);

            //this.PlayPrevious = ReactiveCommand
            //    .CreateFromTask(async () =>
            //    {
            //        await this._playbackService.StopAsync();
            //        var next = this._playbackQueue.Remove();
            //        if (next != null)
            //        {
            //            await this._playbackService.LoadAsync(next);
            //            await this._playbackService.PlayAsync();
            //        }
            //    }, Observable.CombineLatest(
            //        this._playbackQueue.Items.Connect().IsEmpty(),
            //        this._playbackService.WhenCanStopChanged,
            //        (isEmpty, canStop) => !isEmpty && canStop))
            //    .DisposeWith(this._disposables);
            //this.PlayNext = ReactiveCommand
            //    .CreateFromTask(async () =>
            //    {
            //        await this._playbackService.StopAsync();
            //        var next = this._playbackQueue.Remove();
            //        if (next != null)
            //        {
            //            await this._playbackService.LoadAsync(next);
            //            await this._playbackService.PlayAsync();
            //        }
            //    }, Observable.CombineLatest(
            //        this._playbackQueue.Items.Connect().IsEmpty(),
            //        this._playbackService.WhenCanStopChanged,
            //        (isEmpty, canStop) => !isEmpty && canStop))
            //    .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private readonly ObservableAsPropertyHelper<float> _volume_OAPH;
        public float Volume
        {
            get => this._volume_OAPH.Value;
            set => this._audioPlaybackEngine.Volume = value;
        }

        private readonly ObservableAsPropertyHelper<bool> _canResume_OAPH;
        public bool CanResume => this._canResume_OAPH.Value;

        private readonly ObservableAsPropertyHelper<bool> _canPause_OAPH;
        public bool CanPause => this._canPause_OAPH.Value;

        private readonly ObservableAsPropertyHelper<bool> _hasLoadedTrack_OAPH;
        public bool HasLoadedTrack => this._hasLoadedTrack_OAPH.Value;

        public PlaybackTimelineViewModel PlaybackTimelineViewModel { get; }

        #endregion

        #region methods

        public override async void CanClose(Action<bool> callback)
        {
            // TODO: handle exceptions
            await this._audioPlaybackEngine.StopAsync()/*.ConfigureAwait(false)*/;
            this._disposables.Dispose();

            base.CanClose(callback);
        }

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> PlayAll { get; }
        public ReactiveCommand<Unit, Unit> Pause { get; }
        public ReactiveCommand<Unit, Unit> Resume { get; }
        public ReactiveCommand<Unit, Unit> Stop { get; }

        //public ReactiveCommand<Unit, Unit> PlayPrevious { get; }
        //public ReactiveCommand<Unit, Unit> PlayNext { get; }

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
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