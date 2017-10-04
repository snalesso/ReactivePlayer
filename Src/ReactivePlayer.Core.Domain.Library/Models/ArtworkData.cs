using Daedalus.ExtensionMethods;
using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Library.Models
{
    public class ArtworkData : Entity<string>
    {
        public ArtworkData(
            string fingerprint,
            IReadOnlyList<byte> data)
            : base(fingerprint)
        {
            this.Fingerprint = fingerprint;
            this.Data = data != null && data.Any() ? data : throw new ArgumentNullException(nameof(data)); // TODO: localize
            //this.MimeType = mimeType;
        }

        public IReadOnlyList<byte> Data { get; }
        public string Fingerprint { get; }
        //public ImageMimeType MimeType { get; }

        protected override void EnsureIsWellFormattedId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id)); // TODO: localize
        }
    }
}