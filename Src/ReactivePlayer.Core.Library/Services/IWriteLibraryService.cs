using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library
{
    public interface IWriteLibraryService
    {
        Task<bool> AddTrack(AddTrackCommand command);
        Task<bool> AddTracks(IReadOnlyList<AddTrackCommand> commands);

        Task<bool> RemoveTrack(RemoveTrackCommand command);
        Task<bool> RemoveTracks(IReadOnlyList<RemoveTrackCommand> commands);

        Task<bool> UpdateTrack(UpdateTrackCommand command);
        Task<bool> UpdateTracks(IReadOnlyList<UpdateTrackCommand> commands);
    }

    public class AddTrackCommand // TODO: make immutable
    {
        #region library metadata

        public DateTime AddedToLibraryDateTime { get; set; }

        #endregion

        #region file info

        public Uri Location { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }

        #endregion

        #region tags  

        public string Title { get; set; }
        public IReadOnlyList<Artist> Performers { get; set; }
        public IReadOnlyList<Artist> Composers { get; set; }
        public TrackAlbumAssociation AlbumAssociation { get; set; }
        public string Lyrics { get; set; }

        #endregion
    }

    public class UpdateTrackCommand : AddTrackCommand
    {
        public Guid Id { get; set; }
    }

    public class RemoveTrackCommand
    {
        public RemoveTrackCommand(Guid trackId)
        {
            this.TrackId = trackId; // TODO: null check
        }

        public Guid TrackId { get; }
    }

    public class UpdateArtistCommand
    {

    }

    public class UpdateAlbumCommand
    {

    }
}