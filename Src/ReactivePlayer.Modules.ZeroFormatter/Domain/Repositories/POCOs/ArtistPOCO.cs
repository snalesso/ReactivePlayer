using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace ReactivePlayer.Domain.Repositories.POCOs
{
    [ZeroFormattable]
    internal class ArtistPOCO
    {
        [Index(0)]
        public string Name { get; set; }
    }
}