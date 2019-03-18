using System;

namespace ReactivePlayer.Core.Library.Services
{
    public class UpdateTrackCommand : AddTrackCommand
    {
        public UpdateTrackCommand(Uri location) : base(location)
        {
        }

        #region library metadata

        //public DateTime AddedToLibraryDateTime { get; set; }
        public bool IsLoved { get; set; }

        #endregion
    }
}