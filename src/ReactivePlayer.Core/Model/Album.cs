using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Model
{
    public class Album
    {
        public Album(string name)
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
                    (this.name != value
                    && string.IsNullOrWhiteSpace(value))
                    ? value
                    : throw new ArgumentNullException(
                        nameof(value),
                        $"An {this.GetType().Name}'s {nameof(Name)} cannot be null."); // TODO: localize
            }
        }

        public IList<Artist> Authors { get; }

        private DateTime? releaseDate;
        public DateTime? ReleaseDate
        {
            get => this.releaseDate;
            set
            {
                this.releaseDate =
                    (value != this.releaseDate
                    && (value == null
                        || value <= DateTime.Now))
                    ? value
                    : throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        $"An {this.GetType().Name}'s {nameof(ReleaseDate)} cannot be in the future."); // TODO: localize
            }
        }

        public uint? TracksCount { get; set; }

        public uint? DiscsCount { get; set; }
    }
}