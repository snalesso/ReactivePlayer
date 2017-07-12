using Daedalus.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Model
{
    public class Playlist : Entity<Guid>
    {
        // TODO: add sorting
        public Playlist(string name, DateTime dateCreated, IEnumerable<Track> tracks)
            : base(Guid.NewGuid())
        {
            this.Name = name.TrimmedOrNull() ?? throw new ArgumentNullException(nameof(dateCreated)); // TODO: localize
            this.DateCreated = dateCreated <= DateTime.Now ? dateCreated : throw new ArgumentOutOfRangeException(nameof(dateCreated)); // TODO: localize

            if (tracks != null)
                this._tracks.AddRange(tracks.Where(t => t != null));
        }

        public Playlist(string name, DateTime dateCreated)
            : this(name, dateCreated, null)
        {
        }

        public string Name { get; }
        public DateTime DateCreated { get; }

        private readonly List<Track> _tracks = new List<Track>();
        public IReadOnlyList<Track> Tracks => this._tracks.AsReadOnly();

        public void AddTrack(Track track) { }

        public void AddTracks(IEnumerable<Track> tracks) { }

        protected override void EnsureIsValidId(Guid id)
        {
            throw new NotImplementedException();
        }

        public class PlaylistId : ValueObject<PlaylistId>
        {
            public override bool Equals(PlaylistId other)
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<object> GetHashCodeIngredients()
            {
                throw new NotImplementedException();
            }
        }
    }
}