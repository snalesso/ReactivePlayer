using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.PLinq;
using DynamicData.Cache;
using ReactivePlayer.Core.Library.Tracks;

namespace ReactivePlayer.Core.Library.Playlists
{
    public class SmartPlaylist : PlaylistBase
    {
        private readonly IObservable<IChangeSet<Track, uint>> _sourceTracks;

        public SmartPlaylist(uint id, uint? parentId, string name, IObservable<IChangeSet<Track, uint>> tracksSource) : base(id, parentId, name)
        {
            this._sourceTracks = tracksSource ?? throw new ArgumentNullException(nameof(tracksSource));
        }

        public override IObservable<IChangeSet<uint, uint>> TrackIds => throw new NotImplementedException();

        public override bool IsTrackIncluded(Track track)
        {
            throw new NotImplementedException();
        }
    }
}
