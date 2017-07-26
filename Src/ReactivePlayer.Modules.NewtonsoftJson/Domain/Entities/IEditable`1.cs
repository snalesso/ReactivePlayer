using ReactivePlayer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Json.Domain.Entities
{
    internal interface IEditable<TEntity>
          where TEntity : Entity
    {
        bool IsDirty { get; set; }
    }
}