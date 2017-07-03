using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.Core.Services
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