using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Entities
{
    public class TracksDirectory : Entity<Guid>
    {
        public TracksDirectory(string id) : base(Guid.NewGuid())
        {
        }

        protected override void EnsureIsWellFormattedId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}