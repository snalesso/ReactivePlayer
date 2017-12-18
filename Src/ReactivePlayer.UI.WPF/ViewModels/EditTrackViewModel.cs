using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Library.Models;
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

            this.FakeEdit = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackVM) =>
                {
                    var commands = new[]
                    {
                        new UpdateTrackCommand()
                        {
                            Id = trackVM.Id,
                            Title = "Diocane"
                        }
                    };
                    await this._writeLibraryService.UpdateTracksAsync(commands);
                });
        }

        #endregion

        #region properties

        public int Id => this._track.Id;

        public string Title => this._track.Title;

        #endregion

        #region methods
        #endregion

        #region commands

        public ReactiveCommand<TrackViewModel, Unit> FakeEdit { get; }

        #endregion
    }
}