using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ReactivePlayer.Core.Library.Models
{
    public class Album : ValueObject<Album>
    {
        #region ctor

        // TODO: update ctor constraints: might not have all tags
        public Album(
            string title,
            IEnumerable<string> authors = null,
            uint? tracksCount = null,
            uint? discsCount = null)
        {
            // TODO: if the null check exception is activated, the shell freezes on load
            this.Title = title.TrimmedOrNull(); // ?? throw new ArgumentNullException(nameof(this.Title), $"An {this.GetType().Name}'s {nameof(this.Title)} cannot be null."); // TODO: localize
            this.Authors = authors.EmptyIfNull().RemoveNullOrWhitespaces().TrimAll().ToImmutableArray();
            this.TracksCount = tracksCount.NullIf(v => v <= 0);
            this.DiscsCount = discsCount.NullIf(v => v <= 0);
        }

        #endregion

        #region properties

        public string Title { get; }
        public IReadOnlyList<string> Authors { get; }
        public uint? TracksCount { get; }
        public uint? DiscsCount { get; }

        #endregion

        #region ValueObject

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Title;
            foreach (var a in this.Authors)
                yield return a;
            yield return this.TracksCount;
            yield return this.DiscsCount;
        }

        #endregion
    }
}