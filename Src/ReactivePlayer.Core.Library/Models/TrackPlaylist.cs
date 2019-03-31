using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Models
{
    public class TrackPlaylist
    {
        public TrackPlaylist(IEnumerable<uint> trackIds) { }

        public IObservableList<uint> TrackIds { get; }
    }
}