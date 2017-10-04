using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Infrastructure.Servicing
{
    // TODO: move to infrastructure project
    public enum ServiceResponseStatusCode
    {
        Success,
        Timeout,
        AccessDenied,
    }
}