using ReactivePlayer.Core.Entities.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Entities
{
    public class Artist : IArtist
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}