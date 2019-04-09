using DynamicData;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    public interface IArtworksCache
    {
        Task Store(Artwork artwork);
        Task Clear();

        IObservableCache<Artwork, uint> Artworks { get; }
    }
}