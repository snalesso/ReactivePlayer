using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Shell;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class ShellViewModel : ReactiveConductor<Caliburn.Micro.IScreen>.Collection.AllActive, IDisposable
    {
        #region constancts & fields

        private readonly IAudioPlaybackEngine _playbackService;
        private readonly IReadLibraryService _readLibraryService;
        //private readonly IWriteLibraryService _writeLibraryService;
        //private readonly IAudioFileInfoProvider _audioFileInfoProvider;
        private readonly IDialogService _dialogService;

        #endregion

        #region ctor

        public ShellViewModel(
            IAudioPlaybackEngine playbackService,
            //IWriteLibraryService writeLibraryService,
            IReadLibraryService readLibraryService,
            IDialogService dialogService,
            LibraryViewModel libraryViewModel,
            PlaybackControlsViewModel playbackControlsViewModel,
            PlaybackHistoryViewModel playbackHistoryViewModel,
            ShellMenuViewModel shellMenuViewModel)
        {
            this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService));
            //this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this.LibraryViewModel = libraryViewModel ?? throw new ArgumentNullException(nameof(libraryViewModel));
            this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel));
            this.PlaybackHistoryViewModel = playbackHistoryViewModel ?? throw new ArgumentNullException(nameof(playbackHistoryViewModel));
            this.ShellMenuViewModel = shellMenuViewModel ?? throw new ArgumentNullException(nameof(shellMenuViewModel));

            this._isEnabled_OAPH = Observable
                .Return(true)
                .ToProperty(this, nameof(this.IsEnabled))
                .DisposeWith(this._disposables);

            this._taskbarProgressState_OAPH = Observable.CombineLatest(
                    this._playbackService.WhenStatusChanged,
                    this._playbackService.WhenDurationChanged,
                    this._playbackService.WhenPositionChanged,
                    (status, duration, position) =>
                    {
                        switch (status)
                        {
                            case PlaybackStatus.Loaded:
                            case PlaybackStatus.PlayedToEnd:
                            case PlaybackStatus.ManuallyInterrupted:
                            case PlaybackStatus.None:
                                return TaskbarItemProgressState.None;

                            case PlaybackStatus.Playing:
                                if (duration.HasValue && position.HasValue)
                                    return TaskbarItemProgressState.Normal;
                                return TaskbarItemProgressState.Indeterminate;

                            case PlaybackStatus.Paused:
                                return TaskbarItemProgressState.Paused;

                            case PlaybackStatus.Loading:
                                return TaskbarItemProgressState.Indeterminate;

                            case PlaybackStatus.Exploded:
                                return TaskbarItemProgressState.Error;

                            default:
                                return TaskbarItemProgressState.None;
                        }
                    })
                .DistinctUntilChanged()
                .ToProperty(this, nameof(this.TaskbarProgressState))
                .DisposeWith(this._disposables);

            this._taskbarProgressValue_OAPH = Observable.CombineLatest(
                    this._playbackService.WhenDurationChanged,
                    this._playbackService.WhenPositionChanged,
                    (duration, position) =>
                    {
                        if (duration.HasValue && position.HasValue)
                            return (position.Value.TotalMilliseconds / duration.Value.TotalMilliseconds);

                        return Double.NaN;
                    })
                .DistinctUntilChanged()
                .ToProperty(this, nameof(this.TaskbarProgressValue))
                .DisposeWith(this._disposables);

            this._playbackService.WhenTrackChanged
                .Subscribe(track => this.UpdateDisplayName(track))
                .DisposeWith(this._disposables);

            this.ActivateItem(this.LibraryViewModel);
            this.ActivateItem(this.PlaybackControlsViewModel);
            this.ActivateItem(this.PlaybackHistoryViewModel);
            this.ActivateItem(this.ShellMenuViewModel);
        }

        #endregion

        #region properties

        public PlaybackControlsViewModel PlaybackControlsViewModel { get; }
        public LibraryViewModel LibraryViewModel { get; }
        public PlaybackHistoryViewModel PlaybackHistoryViewModel { get; }
        public ShellMenuViewModel ShellMenuViewModel { get; }

        private readonly ObservableAsPropertyHelper<bool> _isEnabled_OAPH;
        public bool IsEnabled => this._isEnabled_OAPH.Value;

        private readonly ObservableAsPropertyHelper<TaskbarItemProgressState> _taskbarProgressState_OAPH;
        public TaskbarItemProgressState TaskbarProgressState => this._taskbarProgressState_OAPH.Value;

        private readonly ObservableAsPropertyHelper<double> _taskbarProgressValue_OAPH;
        public double TaskbarProgressValue => this._taskbarProgressValue_OAPH.Value;

        #endregion

        #region methods

        private void UpdateDisplayName(Track track)
        {
            var shellViewTitle = string.Empty;

            if (track != null)
            {
                shellViewTitle += track.Title;
                if (track.Performers != null)
                {
                    shellViewTitle += " - ";
                    shellViewTitle += string.Join(", ", track.Performers);
                }
            }
            if (shellViewTitle.Length > 0)
                shellViewTitle += " - ";

            shellViewTitle += nameof(ReactivePlayer);

            this.DisplayName = shellViewTitle;
        }

        public override void CanClose(Action<bool> callback)
        {
            //this._playbackService.StopAsync().Wait(); // TODO: handle special cases: playback stop/other actions before closing fail so can close should return false
            base.CanClose(callback);
        }

        #endregion

        #region commands

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