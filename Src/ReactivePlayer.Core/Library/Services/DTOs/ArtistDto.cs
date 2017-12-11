using ReactivePlayer.Core.Library.Models;

namespace ReactivePlayer.Core.Library
{
    public class ArtistDto
    {
        public ArtistDto() { }

        public ArtistDto(Artist artist)
        {
            this.Name = artist.Name;
        }

        public string Name { get; }
    }
}