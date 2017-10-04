using Daedalus.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Infrastructure.Servicing
{
    public class ServiceResponse<TContent, TContentError>
    {
        public ServiceResponse(ServiceResponseStatusCode status, TContent content, IEnumerable<TContentError> errors)
        {
            this.Status = status;
            this.Content = content;
            this.Errors = errors?.ToList().AsReadOnly();
        }

        public ServiceResponse(ServiceResponseStatusCode status, TContent result) : this(status, result, null)
        {
        }

        public ServiceResponseStatusCode Status { get; }

        public TContent Content { get; }

        public IReadOnlyList<TContentError> Errors { get; }

        public bool HasErrors => this.Errors?.Any() ?? false;
    }
}