using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Model
{
    public class TracksDirectory : Entity<Guid>
    {
        public TracksDirectory(string id) : base(Guid.NewGuid())
        {
        }

        protected override void EnsureIsValidId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}