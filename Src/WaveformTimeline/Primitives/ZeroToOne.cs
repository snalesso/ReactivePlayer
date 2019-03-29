using System;
using System.ComponentModel;

namespace WaveformTimeline.Primitives
{
    [TypeConverter(typeof(ZeroToOneTypeConverter))]
    public struct ZeroToOne : IComparable<ZeroToOne>, IEquatable<ZeroToOne>
    {
        public ZeroToOne(double value)
        {
            this._value = double.IsNaN(value) ? 0 : value;
        }

        public ZeroToOne(string value) : this(double.Parse(value))
        {
        }

        private readonly double _value;

        public double Value()
        {
            return Math.Max(0, Math.Min(1, this._value));
        }

        public static implicit operator double(ZeroToOne zto)
        {
            return zto.Value();
        }

        public static implicit operator ZeroToOne(double value)
        {
            return new ZeroToOne(value);
        }

        public int CompareTo(ZeroToOne other)
        {
            return this.Value().CompareTo(other.Value());
        }

        public static bool operator ==(ZeroToOne one, ZeroToOne two)
        {
            return one.Value().Equals(two.Value());
        }

        public static bool operator !=(ZeroToOne one, ZeroToOne two)
        {
            return !(one == two);
        }

        public override bool Equals(object obj)
        {
            return obj is ZeroToOne x && x.Value().Equals(this.Value());
        }

        public bool Equals(ZeroToOne other)
        {
            return this.Value().Equals(other.Value());
        }

        public override int GetHashCode()
        {
            return this.Value().GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.Value()}";
        }
    }
}