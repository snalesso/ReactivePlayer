using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Model.Contracts
{
    public interface IArtwork
    {
        uint Checksum { get; }
        byte[] Data { get; }
        string ToString();
    }
}