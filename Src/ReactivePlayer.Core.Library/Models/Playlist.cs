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
            string name,
            IEnumerable<uint> ids) : base(id)
        {
            this._trackIdsList = new SourceCache<uint, uint>(x=>x);
            this._trackIdsList.Edit(cache => cache.AddOrUpdate(ids));

            this.Name = name;
        }

        private string _name;
        public string Name
        {
            get { return this._name; }
            internal set { this.SetAndRaiseIfChanged(ref this._name, value); }
        }

        private readonly SourceCache<uint, uint> _trackIdsList;
        public IObservableCache<uint, uint> TrackIds => this._trackIdsList;

        protected override void EnsureIsWellFormattedId(uint id)
        {
            if (id.Equals(uint.MinValue))
                // TODO: create ad-hoc exception (e.g. InvalidIdValueException)
                throw new ArgumentException($"{this.GetType().FullName}.{nameof(this.Id)} cannot be set to {id}.", nameof(id));
        }
    }
}