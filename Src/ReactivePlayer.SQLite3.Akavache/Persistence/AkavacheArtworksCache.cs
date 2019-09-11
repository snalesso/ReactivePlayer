using DynamicData;
using ReactivePlayer.Core.Library.Tracks;
using System;
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