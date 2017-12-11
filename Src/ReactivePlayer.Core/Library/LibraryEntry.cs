using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Models
{
    public abstract class LibraryEntry : Entity<Guid>
    {
        public LibraryEntry(
            Guid id,
            DateTime addedToLibraryDateTime,
            bool isLoved,
            IReadOnlyList<DateTime> playedHistory,
            LibraryEntryFileInfo fileInfo) : base(id)
        {
            this.AddedToLibraryDateTime = addedToLibraryDateTime <= DateTime.Now ? addedToLibraryDateTime : throw new ArgumentOutOfRangeException(nameof(addedToLibraryDateTime)); // TODO: localize
            this.IsLoved = isLoved;
            this.PlayedHistory = playedHistory;
            this.FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo)); // TODO: localize
        }

        public DateTime AddedToLibraryDateTime { get; }
        public bool IsLoved { get; }
        public IReadOnlyList<DateTime> PlayedHistory { get; }
        public LibraryEntryFileInfo FileInfo { get; }

        public void LogPlayed()
        {
            throw new NotImplementedException();
        }

        public void UpdateFileInfo(LibraryEntryFileInfo fileInfo)
        {
            throw new NotImplementedException();
        }
    }
}