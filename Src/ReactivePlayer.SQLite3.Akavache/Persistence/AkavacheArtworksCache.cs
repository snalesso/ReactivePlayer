using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.SQLite3.Akavache.Persistence
{
    public class AkavacheArtworksCache : IArtworksCache
    {
        public AkavacheArtworksCache()
        {

        }

        public IObservableCache<Artwork, uint> Artworks => throw new NotImplementedException();

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public Task Store(Artwork artwork)
        {
            throw new NotImplementedException();
        }
    }
}