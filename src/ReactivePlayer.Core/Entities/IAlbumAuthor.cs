using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Entities
{
    public interface IAlbumAuthor
    {
        long AlbumId { get; set; }
        long ArtistId { get; set; }
        short Order { get; set; }
    }
}