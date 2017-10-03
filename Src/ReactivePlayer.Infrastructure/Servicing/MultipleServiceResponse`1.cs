using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Infrastructure.Servicing
{
    public class MultipleServiceResponse<TContent, TContentError>
    {
        public MultipleServiceResponse(IReadOnlyList<ServiceResponse<TContent, TContentError>> results)
        {
            this.Results = results;
        }

        public IReadOnlyList<ServiceResponse<TContent, TContentError>> Results { get; }

        public bool HasErrors => this.Results != null || this.Results.Any(r => r.HasErrors); // TODO: is null-check needed?
    }
}