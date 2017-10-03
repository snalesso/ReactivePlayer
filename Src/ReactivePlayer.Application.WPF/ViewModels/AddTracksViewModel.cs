using ReactivePlayer.Core.Data.Library;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Application.WPF.ViewModels
{
    public class AddTracksViewModel : ReactiveObject
    {
        #region constants & fields

        private readonly IWriteLibraryService _writeLibraryService;

        #endregion

        #region constructors

        public AddTracksViewModel(IWriteLibraryService writeLibraryService)
        {
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));


        }

        #endregion

        #region properties



        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}