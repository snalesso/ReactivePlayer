using ReactivePlayer.Core.Model.Contracts;
using System.Collections.Generic;
using System;

namespace ReactivePlayer.Core.Model
{
    public class Artist
    {
        public Artist(string name)
        {
            this.Name = name;
        }

        private string name;
        public string Name
        {
            get => this.name;
            set
            {
                this.name =
                    (this.name != value && string.IsNullOrWhiteSpace(value))
                    ? value
                    : throw new ArgumentNullException(
                        nameof(value),
                        $"An {this.GetType().Name}'s {nameof(Name)} cannot be null."); // TODO: localize
            }
        }

        //public IReadOnlyList<Track> Singles { get; } = new List<Track>();

        //public IReadOnlyList<Album> Albums { get; } = new List<Album>();

        // TODO: review property/method set as DDD
        public bool Rename(string newName)
        {
            this.Name = newName;

            return true;
        }
    }
}