using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core
{
    // TODO: move to infrastructure project
    public enum ServiceResponseErrorCode
    {
        Unknown = -1,
        Timeout = 0,
        DataAccessFailed
    }
}