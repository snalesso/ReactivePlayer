using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core
{
    public class ServiceResponseError<T>
    {
        public ServiceResponseError(string propertyName, ServiceResponseErrorCode code)
        {
            this.PropertyName = propertyName;
            this.Code = code;
        }

        // TODO: use PropertyInfo instead of string?
        public string PropertyName { get; }

        // TODO: or this might be a string Message { get; }
        public ServiceResponseErrorCode Code { get; }
    }
}