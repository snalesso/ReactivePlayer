using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data
{
    public class ArtistDto
    {
        public ArtistDto(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}