using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Infrastructure.Servicing
{
    public class ServiceResponseError<TErroredType>
    {
        public ServiceResponseError(/*string propertyName,*/ ServiceResponseStatusCode code)
        {
            //this.PropertyName = propertyName;
            this.Code = code;
        }

        //// TODO: use PropertyInfo instead of string?
        //public string PropertyName { get; }

        // TODO: or this might be a string Message { get; }
        public ServiceResponseStatusCode Code { get; }
    }
}