using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public sealed class EditTrackViewModel : ReactiveObject
    {
        #region constants & fields

        private readonly IReadLibraryService _readLibraryService;
        private readonly IWriteLibraryService _writeLibraryService;
        private readonly Track _track;

        #endregion

        #region constructors

        public EditTrackViewModel(
            Track track,
            IReadLibraryService readLibraryService,
            IWriteLibraryService writeLibraryService)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track)); // TODO: localize
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService)); // TODO: localize
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService)); // TODO: localize

            this.EditTrackTagsViewModel = new EditTrackTagsViewModel(this._track);

            this.FakeEdit = ReactiveCommand.Create(
                (TrackViewModel trackVM) =>
                {
                    throw new NotImplementedException();
                });
        }

        #endregion

        #region properties

        public EditTrackTagsViewModel EditTrackTagsViewModel { get; }

        #endregion

        #region methods
        #endregion

        #region commands

        public ReactiveCommand<TrackViewModel, Unit> FakeEdit { get; }

        #endregion
    }
}