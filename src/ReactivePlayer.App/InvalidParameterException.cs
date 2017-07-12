using System;
using System.Reflection;

namespace ReactivePlayer.Core.Model
{
    public class InvalidParameterException<TParam> : Exception
    {
        public InvalidParameterException(TParam invalidValue, PropertyInfo targetProperty)
        {
            this.InvalidValue = invalidValue;
            this.TargetProperty = targetProperty;
        }

        public TParam InvalidValue { get; }

        public PropertyInfo TargetProperty { get; }
    }
}