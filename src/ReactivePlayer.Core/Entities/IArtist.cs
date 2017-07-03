using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Entities
{
    public interface IArtist
    {
        long Id { get; set; }
        string Name { get; set; }
    }
}