using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Model.Contracts
{
    public interface ILibrary
    {
        IList<ITrack> Tracks { get; }
        IReadOnlyList<ITrack> Artists { get; }
        IReadOnlyList<ITrack> Albums { get; }
        IList<IPlaylist> Playlists { get; }
    }
}