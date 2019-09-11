using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Tracks
{
    public interface ITrackFactory
    {
        Task<Track> CreateAsync(Func<uint, Track> trackFactoryMethod);
        //Task<Track> CreateAsync(
        //    Uri location,
        //    TimeSpan? duration,
        //    DateTime? lastModified,
        //    uint? fileSizeBytes,
        //    // Track
        //    string title,
        //    IEnumerable<string> performers,
        //    IEnumerable<string> composers,
        //    uint? year,
        //    TrackAlbumAssociation albumAssociation);
    }
}