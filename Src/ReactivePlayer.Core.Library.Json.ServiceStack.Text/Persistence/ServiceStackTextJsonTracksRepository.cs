using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Json.ServiceStack.Text.Persistence
{
    public sealed class ServiceStackTextJsonTracksRepository : SerializedEntityRepository<Track, uint>, ITracksRepository
    {
        private const string DBFileName = "tracks.json";

        public ServiceStackTextJsonTracksRepository()
            :base(Path.Combine(Assembly.GetEntryAssembly().Location, "db", DBFileName))
        {
        }

        protected override bool IsDeserialized => throw new NotImplementedException();

        public Track CreateTracksAsync(Uri location, TimeSpan? duration, DateTime? lastModified, uint? fileSizeBytes, DateTime addedToLibraryDateTime, bool isLoved, string title, IEnumerable<Artist> performers, IEnumerable<Artist> composers, uint? year, TrackAlbumAssociation albumAssociation)
        {
            throw new NotImplementedException();
        }

        protected override Task DeserializeCore()
        {
            throw new NotImplementedException();
        }

        protected override Task SerializeCore()
        {
            throw new NotImplementedException();
        }
    }
}
