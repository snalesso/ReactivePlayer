namespace ReactivePlayer.Core.Library.Tracks
{
    public class RemoveTrackCommand
    {
        public RemoveTrackCommand(uint id)
        {
            this.Id = id;
        }

        public uint Id { get; }
    }
}