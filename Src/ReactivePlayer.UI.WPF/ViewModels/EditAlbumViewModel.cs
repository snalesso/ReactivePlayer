using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class EditAlbumViewModel : ReactiveScreen
    {
        #region constants & fields

        private readonly Album _album;

        #endregion

        #region ctors

        public EditAlbumViewModel(Album album)
        {
            this._album = album ?? throw new ArgumentNullException(nameof(album));

            this.Title = this._album.Title;
            this.Authors = this._album.Authors;
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
                this.RaiseAndSetIfChanged(ref this._title, value);
                this.GetAlbum();
            }
        }

        private IReadOnlyList<string> _authors;
        public IReadOnlyList<string> Authors
        {
            get { return this._authors; }
            set { this.RaiseAndSetIfChanged(ref this._authors, value); }
        }

        private uint? _tracksCount;
        public uint? TracksCount
        {
            get { return this._tracksCount; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._tracksCount, value);
                this.GetAlbum();
            }
        }

        private uint? _discsCount;
        public uint? DiscsCount
        {
            get { return this._discsCount; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._discsCount, value);
                this.GetAlbum();
            }
        }

        #endregion

        #region methods

        public Album GetAlbum()
        {
            return new Album(
                this.Title,
                this.Authors,
                this.TracksCount,
                this.DiscsCount);
        }

        #endregion

        #region commands
        #endregion
    }
}
