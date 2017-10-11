using ReactivePlayer.Core.Application.Library;
using ReactiveUI;
using System;

namespace ReactivePlayer.Presentation.WPF.ViewModels
{
    public class AddTracksViewModel : ReactiveObject
    {
        #region constants & fields

        private readonly IWriteLibraryService _writeLibraryService;

        #endregion

        #region constructors

        public AddTracksViewModel(IWriteLibraryService writeLibraryService) => this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));

        #endregion

        #region properties



        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}