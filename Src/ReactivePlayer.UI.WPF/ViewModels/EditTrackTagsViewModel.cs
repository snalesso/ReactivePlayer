using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class EditTrackTagsViewModel : ReactiveScreen
    {
        #region constants & fields

        private readonly Track _track;
        private readonly Func<IEnumerable<string>, EditArtistsViewModel> _editArtistsViewModelViewModelFactoryMethod;
        private readonly Func<TrackAlbumAssociation, EditTrackAlbumAssociationViewModel> _editTrackAlbumAssociationViewModelFactoryMethod;
        //private readonly EditTrackCommand _editTrackCommand;

        #endregion

        #region ctors

        public EditTrackTagsViewModel(
            Track track,
            Func<IEnumerable<string>, EditArtistsViewModel> editArtistsViewModelViewModelFactoryMethod,
            Func<TrackAlbumAssociation, EditTrackAlbumAssociationViewModel> editTrackAlbumAssociationViewModelFactoryMethod)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track));
            this._editArtistsViewModelViewModelFactoryMethod = editArtistsViewModelViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editArtistsViewModelViewModelFactoryMethod));
            this._editTrackAlbumAssociationViewModelFactoryMethod = editTrackAlbumAssociationViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editTrackAlbumAssociationViewModelFactoryMethod));

            this.Title = this._track.Title;
            this.EditPerformersViewModel = this._editArtistsViewModelViewModelFactoryMethod.Invoke(this._track.Performers);
            this.EditComposersViewModel = this._editArtistsViewModelViewModelFactoryMethod.Invoke(this._track.Composers);
            this.Year = this._track.Year;

            this.EditTrackAlbumAssociationViewModel = this._editTrackAlbumAssociationViewModelFactoryMethod.Invoke(this._track.AlbumAssociation);

            this.CancelAndClose = ReactiveCommand.Create(
                () =>
                {
                    this.TryClose(false);
                }
                ).DisposeWith(this._disposables);

            this.ConfirmAndClose= ReactiveCommand.Create(
                () =>
                {
                    this.TryClose(true);
                }
                ).DisposeWith(this._disposables);

            this.DisplayName = "Edit";
        }

        #endregion

        #region properties

        private string _title;
        public string Title
        {
            get => this._title;
            set => this.RaiseAndSetIfChanged(ref this._title, value);
        }

        public EditArtistsViewModel EditPerformersViewModel { get; }
        public EditArtistsViewModel EditComposersViewModel { get; }

        private uint? _year;
        public uint? Year
        {
            get => this._year;
            set => this.RaiseAndSetIfChanged(ref this._year, value);
        }

        public EditTrackAlbumAssociationViewModel EditTrackAlbumAssociationViewModel { get; }

        #endregion

        #region methods
        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> CancelAndClose { get; }
        public ReactiveCommand<Unit, Unit> ConfirmAndClose { get; }

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this._isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}