﻿using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Tracks;
using System;
using System.Collections.ObjectModel;

namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public class EditTrackAlbumAssociationViewModel : ReactiveScreen
    {
        #region constants & fields

        private readonly TrackAlbumAssociation _trackAlbumAssociation;

        #endregion

        #region ctors

        public EditTrackAlbumAssociationViewModel(
            TrackAlbumAssociation trackAlbumAssociation)
        {
            this._trackAlbumAssociation = trackAlbumAssociation ?? throw new ArgumentNullException(nameof(trackAlbumAssociation));

            this.TrackNumber = this._trackAlbumAssociation.TrackNumber;
            this.DiscNumber = this._trackAlbumAssociation.DiscNumber;

            this._albumViewModels = new ObservableCollection<AlbumViewModel>();
            this.AlbumViewModels = new ReadOnlyObservableCollection<AlbumViewModel>(this._albumViewModels);
        }

        #endregion

        #region properties

        private readonly ObservableCollection<AlbumViewModel> _albumViewModels;
        public ReadOnlyObservableCollection<AlbumViewModel> AlbumViewModels { get; }

        private AlbumViewModel _albumViewModel;
        public AlbumViewModel AlbumViewModel
        {
            get { return this._albumViewModel; }
            set { this.Set(ref this._albumViewModel, value); }
        }

        private uint? _trackNumber;
        public uint? TrackNumber
        {
            get { return this._trackNumber; }
            set { this.Set(ref this._trackNumber, value); }
        }

        private uint? _discNumber;
        public uint? DiscNumber
        {
            get { return this._discNumber; }
            set { this.Set(ref this._discNumber, value); }
        }

        #endregion

        #region methods

        private TrackAlbumAssociation GetTrackAlbumAssociation()
        {
            return new TrackAlbumAssociation(
                this.AlbumViewModel.Album,
                this.TrackNumber,
                this.DiscNumber);
        }

        #endregion

        #region commands
        #endregion
    }
}