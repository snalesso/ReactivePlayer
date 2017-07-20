using ReactivePlayer.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Repositories
{
    public interface IArtistsRepository : IValueObjectRepository<Artist, Track>
    {
    }
}