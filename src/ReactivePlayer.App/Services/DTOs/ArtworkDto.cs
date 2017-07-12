using ReactivePlayer.Domain.Model;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.App.Services.DTOs
{
    public class ArtworkDto
    {
        public ArtworkDto(IEnumerable<byte> data, ArtworkType type)
        {
            this.Data = data ?? throw new ArgumentNullException(nameof(data)); // TODO: localize
            this.Type = type;
        }

        public IEnumerable<byte> Data { get; }
        public ArtworkType Type { get; }
    }
}