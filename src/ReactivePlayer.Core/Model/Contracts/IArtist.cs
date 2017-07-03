using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Model.Contracts
{
    public interface IArtist
    {
        string Name { get; }
        IList<ITrack> Tracks { get; }
        IList<IAlbum> Albums { get; }
    }
}