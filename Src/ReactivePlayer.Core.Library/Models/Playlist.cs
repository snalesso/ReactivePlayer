using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Models
{
    public class Playlist
    {
        public Playlist(IEnumerable<uint> ids) {

        }

        private SourceList<uint> _trackIdsList;
        public IObservableList<uint> TrackIds { get; }
    }
}