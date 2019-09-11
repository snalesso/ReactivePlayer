using DynamicData;
using ReactivePlayer.Core.Library.Tracks;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Tracks
{
    public interface IArtworksCache
    {
        Task Store(Artwork artwork);
        Task Clear();

        IObservableCache<Artwork, uint> Artworks { get; }
    }
}