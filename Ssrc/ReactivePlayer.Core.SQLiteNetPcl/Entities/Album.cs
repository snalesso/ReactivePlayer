using ReactivePlayer.Core.Entities.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Entities
{
    public class Album
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ReleaseDateTimeTicks { get; set; }
        public short? TracksCount { get; set; }
        public short? DiscsCount { get; set; }
    }
}