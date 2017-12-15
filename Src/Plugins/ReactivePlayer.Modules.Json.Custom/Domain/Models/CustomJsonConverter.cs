using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Models
{
    public abstract class CustomJsonConverter<T>
        where T : class
    {
        public abstract string ToJson(T realObject, StringBuilder stringBuilder);
        public abstract T ToObject(string jsonObject);
    }
}