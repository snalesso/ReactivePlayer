using ReactivePlayer.Core.Domain.Library.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Application.Services.Library
{
    public class ArtworkDto
    {
        public ArtworkDto(IEnumerable<byte> data, ArtworkType type)
        {
            this.Data = data ?? throw new ArgumentNullException(nameof(data)); // TODO: localize
            this.Type = type;
        }

        public IEnumerable<byte> Data { get; }
        public ArtworkType Type { get; } // TODO: is this really needed?
    }
}