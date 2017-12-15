using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Models
{
    public abstract class LibraryEntry : Entity<Guid>
    {
        #region ctor

        public LibraryEntry(
            Guid id,
            DateTime addedToLibraryDateTime,
            bool isLoved,
            IReadOnlyList<DateTime> playedHistory,
            LibraryEntryFileInfo fileInfo)
            : base(id)
        {
            this.AddedToLibraryDateTime = addedToLibraryDateTime <= DateTime.Now ? addedToLibraryDateTime : throw new ArgumentOutOfRangeException(nameof(addedToLibraryDateTime)); // TODO: localize
            this.IsLoved = isLoved;
            this.PlayedHistory = playedHistory;
            this.FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo)); // TODO: localize
        }

        #endregion

        #region properties

        public DateTime AddedToLibraryDateTime { get; protected set; }
        public bool IsLoved { get; protected set; }
        public IReadOnlyList<DateTime> PlayedHistory { get; protected set; }
        public LibraryEntryFileInfo FileInfo { get; protected set; }

        #endregion

        #region methods

        public void LogPlayed()
        {
            throw new NotImplementedException();
        }

        public void UpdateFileInfo(LibraryEntryFileInfo fileInfo)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}