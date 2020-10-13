using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public class EditAlbumViewModel : ReactiveScreen
    {
        #region constants & fields

        private readonly Album _album;
        private readonly Func<IEnumerable<string>, EditArtistsViewModel> _editArtistsViewModelViewModelFactoryMethod;

        #endregion

        #region ctors

        public EditAlbumViewModel(
            Album album,
            Func<IEnumerable<string>, EditArtistsViewModel> editArtistsViewModelViewModelFactoryMethod)
        {
            this._album = album ?? throw new ArgumentNullException(nameof(album));
            this._editArtistsViewModelViewModelFactoryMethod = editArtistsViewModelViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editArtistsViewModelViewModelFactoryMethod));

            this.Title = this._album.Title;
            this.EditAuthorsViewModel = this._editArtistsViewModelViewModelFactoryMethod.Invoke(this._album.Authors);
            this.TracksCount = this._album.TracksCount;
            this.DiscsCount = this._album.DiscsCount;
        }

        #endregion

        #region properties

        private string _title;
        public string Title
        {
            get { return this._title; }
            set
            {
                this.Set(ref this._title, value);
                this.GetAlbum();
            }
        }

        public EditArtistsViewModel EditAuthorsViewModel { get; }

        private uint? _tracksCount;
        public uint? TracksCount
        {
            get { return this._tracksCount; }
            set
            {
                this.Set(ref this._tracksCount, value);
                this.GetAlbum();
            }
        }

        private uint? _discsCount;
        public uint? DiscsCount
        {
            get { return this._discsCount; }
            set
            {
                this.Set(ref this._discsCount, value);
                this.GetAlbum();
            }
        }

        #endregion

        #region methods

        public Album GetAlbum()
        {
            return new Album(
                this.Title,
                this.EditAuthorsViewModel.EditArtistViewModels.Select(x => x.ArtistName).ToArray(),
                this.TracksCount,
                this.DiscsCount);
        }

        #endregion

        #region commands
        #endregion
    }
}
