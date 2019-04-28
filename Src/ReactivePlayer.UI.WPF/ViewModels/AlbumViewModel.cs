using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class AlbumViewModel : ReactiveScreen
    {
        #region constants & fields

        //private readonly Album _album;

        #endregion

        #region ctors

        public AlbumViewModel(
            Album album)
        {
            this.Album = album ?? throw new ArgumentNullException(nameof(album));
        }

        #endregion

        #region properties

        public Album Album { get; }

        public string Title => this.Album.Title;
        public IReadOnlyList<string> Authors => this.Album.Authors;
        public uint? TracksCount => this.Album.TracksCount;
        public uint? DiscsCount => this.Album.DiscsCount;

        #endregion

        #region methods

        #endregion

        #region commands
        #endregion
    }
}