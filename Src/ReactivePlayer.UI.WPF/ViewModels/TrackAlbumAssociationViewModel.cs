﻿using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Tracks;
using System;

namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public class TrackAlbumAssociationViewModel : ReactiveScreen
    {
        #region constants & fields

        private readonly TrackAlbumAssociation _trackAlbumAssociation;

        #endregion

        #region ctors

        public TrackAlbumAssociationViewModel(
            TrackAlbumAssociation trackAlbumAssociation)
        {
            this._trackAlbumAssociation = trackAlbumAssociation ?? throw new ArgumentNullException(nameof(trackAlbumAssociation));

            if (this._trackAlbumAssociation.Album != null)
                this.AlbumViewModel = new AlbumViewModel(this._trackAlbumAssociation.Album);
        }

        #endregion

        #region properties

        public AlbumViewModel AlbumViewModel { get; }

        public uint? TrackNumber => this._trackAlbumAssociation.TrackNumber;

        public uint? DiscNumber => this._trackAlbumAssociation.DiscNumber;

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}