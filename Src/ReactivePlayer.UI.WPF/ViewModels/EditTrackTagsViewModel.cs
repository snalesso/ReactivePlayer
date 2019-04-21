using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class EditTrackTagsViewModel : ReactiveScreen
    {
        private readonly Track _track;
        //private readonly EditTrackCommand _editTrackCommand;

        public EditTrackTagsViewModel(Track track)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track));

            //this._editTrackCommand = new EditTrackCommand()
        }

        private string _title;
        public string Title
        {
            get => this._title;
            set => this.SetAndRaiseIfChanged(ref this._title, value);
        }

        private IReadOnlyList<string> _performers;
        public IReadOnlyList<string> Performers
        {
            get => this._performers;
            set => this.SetAndRaiseIfChanged(ref this._performers, value);
        }

        private IReadOnlyList<string> _composers;
        public IReadOnlyList<string> Composers
        {
            get => this._composers;
            set => this.SetAndRaiseIfChanged(ref this._composers, value);
        }

        private uint? _year;
        public uint? Year
        {
            get => this._year;
            set => this.SetAndRaiseIfChanged(ref this._year, value);
        }

        private TrackAlbumAssociation _albumAssociation;
        public TrackAlbumAssociation AlbumAssociation
        {
            get => this._albumAssociation;
            set => this.SetAndRaiseIfChanged(ref this._albumAssociation, value);
        }
    }
}