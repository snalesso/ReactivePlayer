using ReactivePlayer.Core.Domain.Library.Models;

namespace ReactivePlayer.Core.Application.Library
{
    public class TrackCriteria
    {
        public string Title { get; set; }
        public string PerformerName { get; set; }
        public string AlbumName { get; set; }

        public bool IsRespectedBy(Track track) => true;
    }
}