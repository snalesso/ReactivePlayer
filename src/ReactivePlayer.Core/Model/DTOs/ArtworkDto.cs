using ReactivePlayer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.DTOs
{
    public class ArtworkDto
    {
        public ArtworkDto(IEnumerable<byte> data, ArtworkType type = ArtworkType.Cover)
        {
            this.Data = data;
            this.Type = type;
            this._checksum = this.Data.GetHashCode().ToString();
        }

        public ArtworkType Type { get; }
        public IEnumerable<byte> Data { get; }
        private readonly string _checksum;
        public string Checksum { get; }
    }
}