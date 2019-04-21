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

        private Album _album;

        #endregion

        #region ctors

        public AlbumViewModel(
            Album album)
        {
            this._album = album ?? throw new ArgumentNullException(nameof(album));

            this.Title = this._album.Title;
            this.TracksCount = this._album.TracksCount;
            this.DiscsCount = this._album.DiscsCount;
        }

        #endregion

        #region properties

        public Album Album { get; }

        //private string _title;
        public string Title { get; }
        //{
        //    get { return this._title; }
        //    set { this.SetAndRaiseIfChanged(ref this._title, value); }
        //}

        //private uint? _tracksCount;
        public uint? TracksCount { get; }
        //{
        //    get { return this._tracksCount; }
        //    set { this.SetAndRaiseIfChanged(ref this._tracksCount, value); }
        //}

        //private uint? _discsCount;
        public uint? DiscsCount { get; }
        //{
        //    get { return this._discsCount; }
        //    set { this.SetAndRaiseIfChanged(ref this._discsCount, value); }
        //}

        #endregion

        #region methods

        #endregion

        #region commands
        #endregion
    }
}