namespace ReactivePlayer.Core.Library
{
    public class RemoveTrackCommand
    {
        public RemoveTrackCommand(int trackId)
        {
            this.TrackId = trackId; // TODO: null check
        }

        public int TrackId { get; }
    }
}