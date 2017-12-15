using ReactivePlayer.Domain.Models;
using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Modules.Json.Newtonsoft.Domain.Models
{
    internal interface IEditable<TEntity>
          where TEntity : Entity
    {
        bool IsDirty { get; set; }
    }
}