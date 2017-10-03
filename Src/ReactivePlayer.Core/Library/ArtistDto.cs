using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data
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