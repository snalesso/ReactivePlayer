using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class EditTrackTagsViewModel : ReactiveScreen
    {
        #region constants & fields

        private readonly Track _track;
        private readonly Func<IEnumerable<string>, EditArtistsViewModel> _editArtistsViewModelViewModelFactoryMethod;
        //private readonly EditTrackCommand _editTrackCommand;

        #endregion

        #region ctors

        public EditTrackTagsViewModel(
            Track track,
            Func<IEnumerable<string>, EditArtistsViewModel> editArtistsViewModelViewModelFactoryMethod)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track));
            this._editArtistsViewModelViewModelFactoryMethod = editArtistsViewModelViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editArtistsViewModelViewModelFactoryMethod));

            //this._editTrackCommand = new EditTrackCommand()
            this.Title = this._track.Title;

            //this._performers = new ObservableCollection<string>(this._track.Performers);
            this.EditPerformersViewModel = this._editArtistsViewModelViewModelFactoryMethod.Invoke(this._track.Performers);

            //this._composers = new ObservableCollection<string>(this._track.Composers);
            //this.Composers = new ReadOnlyObservableCollection<string>(this._composers);

            //this.Year = this._track.Year;
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

        //public UniqueStringsListEditorViewModel PerformersEditor { get; }
        ////{
        ////    get => this._performers;
        ////    set => this.SetAndRaiseIfChanged(ref this._performers, value);
        ////}

        //private readonly ObservableCollection<string> _composers;
        //public ReadOnlyObservableCollection<string> Composers { get; }
        ////{
        ////    get => this._composers;
        ////    set => this.SetAndRaiseIfChanged(ref this._composers, value);
        ////}

        //private uint? _year;
        //public uint? Year
        //{
        //    get => this._year;
        //    set => this.RaiseAndSetIfChanged(ref this._year, value);
        //}

        //private EditTrackAlbumAssociationViewModel _editTrackAlbumAssociationViewModel;
        //public EditTrackAlbumAssociationViewModel EditTrackAlbumAssociationViewModel
        //{
        //    get => this._editTrackAlbumAssociationViewModel;
        //    private set => this.RaiseAndSetIfChanged(ref this._editTrackAlbumAssociationViewModel, value);
        //}

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}