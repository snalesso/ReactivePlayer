using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library
{
    public interface IWriteLibraryService
    {
        Task<bool> AddTracks(IReadOnlyList<AddTrackCommand> addTrackCommands);
        Task<bool> RemoveTracks(IReadOnlyList<Track> tracks);
        Task<bool> UpdateTracks(IReadOnlyList<UpdateTrackCommand> updateTrackCommands);
    }

    public class AddTrackCommand
    {
        #region library metadata

        public DateTime AddedToLibraryDateTime { get; }

        #endregion

        #region track file info

        public Uri Location { get; }
        public TimeSpan? Duration { get; }
        public DateTime? LastModifiedDateTime { get; }

        #endregion

        #region tags

        public TrackTagsDto Tags { get; }

        #endregion
    }

    public class RemoveTrackCommand
    {
        public RemoveTrackCommand(Guid trackId)
        {
            this.TrackId = trackId;
        }

        public Guid TrackId { get; }
    }

    public class UpdateTrackCommand
    {

    }

    public class UpdateArtistCommand
    {

    }

    public class UpdateAlbumCommand
    {

    }
}