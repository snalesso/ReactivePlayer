using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Services
{
    public class UpdatedEvent<T>
    {
        public UpdatedEvent(T oldData, T newData)
        {
            this.OldData = oldData;
            this.NewData = newData;
        }

        public T OldData { get; }
        public T NewData { get; }
    }
}