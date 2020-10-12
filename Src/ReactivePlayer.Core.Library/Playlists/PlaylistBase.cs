using DynamicData;
using ReactivePlayer.Core.Domain.Models;
using ReactivePlayer.Core.Library.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Playlists
{
    public abstract class PlaylistBase : Entity<uint>
    {
        public PlaylistBase(
            uint id,
            uint? parentId,
            string name) : base(id)
        {
            this.Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));

            this.ParentId = parentId;
        }

        private string _name;
        public string Name
        {
            get { return this._name; }
            internal set { this.SetAndRaiseIfChanged(ref this._name, value); }
        }

        private uint? _parentId;
        public uint? ParentId
        {
            get { return this._parentId; }
            internal set { this.SetAndRaiseIfChanged(ref this._parentId, value); }
        }

        // TODO: consider & maybe benchmark exposing a cache, which might improve performance a lot, or consider exposing a .HasKey(uint id) method
        [Obsolete]
        public abstract IObservable<IChangeSet<uint, uint>> TrackIds { get; }

        public abstract bool IsTrackIncluded(Track track);
    }
}