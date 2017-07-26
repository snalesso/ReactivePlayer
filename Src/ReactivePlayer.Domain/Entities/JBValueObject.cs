using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Model
{
    public abstract class JBValueObject<T> : IEquatable<T>
        where T : JBValueObject<T>
    {
        private IEnumerable<FieldInfo> GetFields()
        {
            var thisType = this.GetType();
            var fields = new List<FieldInfo>();

            while (thisType != typeof(object))
            {
                fields.AddRange(thisType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                thisType = thisType.BaseType;
            }

            return fields;
        }

        public abstract bool Equals(T other);
        //{
        //    if (other == null)
        //        return false;

        //    var thisType = this.GetType();
        //    if (thisType != other.GetType())
        //        return false;

        //    FieldInfo[] fields = thisType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        //    foreach (FieldInfo field in fields)
        //    {
        //        object value1 = field.GetValue(other);
        //        object value2 = field.GetValue(this);

        //        if (value1 == null)
        //        {
        //            if (value2 != null)
        //                return false;
        //        }
        //        else if (!value1.Equals(value2))
        //            return false;
        //    }

        //    return true;
        //}

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return this.Equals(obj as T);
        }
        
        public override int GetHashCode()
        {
            int startValue = 17;
            int multiplier = 59;
            int hashCode = startValue;
            var fields = this.GetFields();

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(this);

                if (value != null)
                    hashCode = hashCode * multiplier + value.GetHashCode();
            }

            return hashCode;
        }

        public static bool operator ==(JBValueObject<T> x, JBValueObject<T> y) => x.Equals(y);

        public static bool operator !=(JBValueObject<T> x, JBValueObject<T> y) => !(x == y);
    }
}