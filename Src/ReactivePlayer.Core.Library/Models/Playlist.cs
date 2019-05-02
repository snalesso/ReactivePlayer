using DynamicData;
using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Models
{
    public class Playlist : Entity<uint>
    {
        public Playlist(
            uint id,
            IEnumerable<uint> ids) : base(id)
        {
            this._trackIdsList = new SourceList<uint>(ObservableChangeSet.Create<uint>(list => () => list.AddRange(ids)));
        }

        private readonly SourceList<uint> _trackIdsList;
        public IObservableList<uint> TrackIds => this._trackIdsList;

        protected override void EnsureIsWellFormattedId(uint id)
        {
            if (id.Equals(uint.MinValue))
                // TODO: create ad-hoc exception (e.g. InvalidIdValueException)
                throw new ArgumentException($"{this.GetType().FullName}.{nameof(this.Id)} cannot be set to {id}.", nameof(id));
        }
    }
}