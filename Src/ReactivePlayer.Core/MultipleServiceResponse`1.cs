using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core
{
    public class MultipleServiceResponse<T>
    {
        public MultipleServiceResponse(IReadOnlyList<ServiceResponse<T>> results)
        {
            this.Results = results;
        }

        public IReadOnlyList<ServiceResponse<T>> Results { get; }

        public bool HasErrors => this.Results != null || this.Results.Any(r => r.HasErrors); // TODO: is null-check needed?
    }
}