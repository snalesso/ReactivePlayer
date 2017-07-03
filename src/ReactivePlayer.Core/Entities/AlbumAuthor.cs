using ReactivePlayer.Core.Entities.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Entities
{
    public class AlbumAuthor : IAlbumAuthor
    {
        public long AlbumId { get; set; }
        public long ArtistId { get; set; }
        public short Order { get; set; }
    }
}