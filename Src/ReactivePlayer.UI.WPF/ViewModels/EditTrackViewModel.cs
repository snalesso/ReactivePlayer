using ReactivePlayer.Core.Library.Tracks;
using ReactiveUI;
using System;
using System.Reactive;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public sealed class EditTrackViewModel : ReactiveObject
    {
        #region constants & fields

        private readonly IReadLibraryService _readLibraryService;
        private readonly IWriteLibraryService _writeLibraryService;
        private readonly Track _track;
        private readonly Func<Track, EditTrackTagsViewModel> _editTrackTagsViewModelFactoryMethod;

        #endregion

        #region constructors

        public EditTrackViewModel(
            IReadLibraryService readLibraryService,
            IWriteLibraryService writeLibraryService,
            Track track,
            Func<Track, EditTrackTagsViewModel> editTrackTagsViewModelFactoryMethod)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track));
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._editTrackTagsViewModelFactoryMethod = editTrackTagsViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editTrackTagsViewModelFactoryMethod));

            this.EditTrackTagsViewModel = this._editTrackTagsViewModelFactoryMethod.Invoke(this._track);

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