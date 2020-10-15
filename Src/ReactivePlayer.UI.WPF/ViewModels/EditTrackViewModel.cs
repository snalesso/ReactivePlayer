using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Tracks;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;

namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public class EditTrackViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IReadLibraryService _readLibraryService;
        private readonly IWriteLibraryService _writeLibraryService;
        private readonly Track _track;
        private readonly Func<Track, EditTrackTagsViewModel> _editTrackTagsViewModelFactoryMethod;

        #endregion

        #region constructors

        public EditTrackViewModel(
            IReadLibraryService readLibraryService,
            IWriteLibraryService writeLibraryService,
            Track track,
            Func<Track, EditTrackTagsViewModel> editTrackTagsViewModelFactoryMethod)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track));
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._editTrackTagsViewModelFactoryMethod = editTrackTagsViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editTrackTagsViewModelFactoryMethod));

            this.EditTrackTagsViewModel = this._editTrackTagsViewModelFactoryMethod.Invoke(this._track);

            this.CancelAndClose = ReactiveCommand.CreateFromTask(() => this.TryCloseAsync(false)).DisposeWith(this._disposables);
            this.CancelAndClose.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex)).DisposeWith(this._disposables);

            this.ConfirmAndClose = ReactiveCommand.CreateFromTask(() => this.TryCloseAsync(true)).DisposeWith(this._disposables);
            this.CancelAndClose.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex)).DisposeWith(this._disposables);

            this.DisplayName = "Edit";
        }

        #endregion

        #region properties

        public EditTrackTagsViewModel EditTrackTagsViewModel { get; }

        #endregion

        #region methods
        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> CancelAndClose { get; }
        public ReactiveCommand<Unit, Unit> ConfirmAndClose { get; }

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