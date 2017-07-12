using Daedalus.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Model
{
    public class ArtworkData : Entity<string>
    {
        public ArtworkData(
            string hash,
            IEnumerable<byte> data)
            : base(hash)
        {
            this.Hash = hash;
            this.Data = data is IReadOnlyList<byte> ? data as IReadOnlyList<byte> : data.ToArray();
            //this.MimeType = mimeType;
        }

        public IReadOnlyList<byte> Data { get; }
        public string Hash { get; }
        //public ImageMimeType MimeType { get; }

        protected override void EnsureIsValidId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id)); // TODO: localize
        }
    }
}