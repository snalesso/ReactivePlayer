using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Tests.Model
{
    public class BaseTestEntity : Entity
    {
        public string Name { get; }

        #region entity

        protected override bool EqualsCore(Entity other)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<object> GetHashCodeComponents()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}