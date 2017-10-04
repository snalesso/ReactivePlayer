using Daedalus.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Models
{
    public class TrackLibraryMetadata
    {
        public TrackLibraryMetadata(
            DateTime addedToLibraryDateTime,
            bool isLoved,
            IReadOnlyList<DateTime> playedHistory)
        {
            this.AddedToLibraryDateTime = addedToLibraryDateTime < DateTime.Now ? addedToLibraryDateTime : throw new ArgumentNullException(nameof(addedToLibraryDateTime));
            this.IsLoved = isLoved;
            this.PlayedHistory = playedHistory;
        }

        public DateTime AddedToLibraryDateTime { get; }
        public bool IsLoved { get; }
        public IReadOnlyList<DateTime> PlayedHistory { get; }
    }
}