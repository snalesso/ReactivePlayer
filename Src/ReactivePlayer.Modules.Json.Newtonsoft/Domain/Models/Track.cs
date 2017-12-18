using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Json.Domain.Entities
{
    internal class Track : Model.Track, IEditable<Model.Track>
    {
        public bool IsDirty { get; set; }
    }
}