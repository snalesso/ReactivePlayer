using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Repositories.POCOs
{
    internal class TrackPOCO
    {
        public Uri Location { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
        public TagsPOCO Tags { get; set; }
        public DateTime AddedToLibraryDateTime { get; set; }
        public bool IsLoved { get; set; }
        public IReadOnlyList<DateTime> PlayedHistory { get; set; }
    }
}