using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Services
{
    public interface INonCryptoHashingService
    {
        Task<IReadOnlyList<byte>> ComputeHashAsync(IEnumerable<byte> data);
        Task<string> ComputeHashAsync(IEnumerable<byte> data, Encoding textEncoding);
    }
}