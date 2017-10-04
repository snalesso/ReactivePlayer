using ReactivePlayer.Core.Domain.Library.Models;

namespace ReactivePlayer.Core.Application.Services.Library
{
    public class ArtistDto
    {
        public ArtistDto() { }

        public ArtistDto(Artist artist)
        {
            this.Name = artist.Name;
        }

        public string Name { get; set; }
    }
}