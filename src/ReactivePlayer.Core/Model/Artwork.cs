using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Model
{
    public class Artwork
    {
        public Artwork(IEnumerable<byte> data, PictureMimeType mimeType, ArtworkType type = ArtworkType.Cover)
        {
            this.Data = data;
            this.Type = type;
            this._checksum = this.Data.GetHashCode().ToString();
        }

        public ArtworkType Type { get; }
        public PictureMimeType MimeType { get; }
        public IEnumerable<byte> Data { get; }
        private readonly string _checksum;
        public string Checksum { get; }
    }
}