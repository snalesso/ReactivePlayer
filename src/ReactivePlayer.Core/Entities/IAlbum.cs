using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Entities
{
    public interface IAlbum
    {
        long Id { get; set; }
        string Name { get; set; }
        long? ReleaseDateTimeTicks { get; set; }
        short? TracksCount { get; set; }
        short? DiscsCount { get; set; }
    }
}