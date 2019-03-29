using System;

namespace WaveformTimeline.Primitives
{
    /// <summary>
    /// A double that is coerced to the provided default value, or zero,
    /// when it happens to be NaN or infinite.
    /// </summary>
    public struct FiniteDouble : IComparable<FiniteDouble>, IEquatable<FiniteDouble>
    {
        public FiniteDouble(double value, double defaultValue = 0.0d)
        {
            this._value = value;
            this._defaultValue = defaultValue;
        }

        private readonly double _value;
        private readonly double _defaultValue;

        private bool IsFinite(double d)
        {
            return !(double.IsNaN(d) || double.IsInfinity(d));
        }

        public double Value()
        {
            return this.IsFinite(this._value)
                ? this._value
                : new FiniteDouble(this._defaultValue, 0.0).Value();
        }

        public static implicit operator double(FiniteDouble fd)
        {
            return fd.Value();
        }

        public static implicit operator FiniteDouble(double value)
        {
            return new FiniteDouble(value);
        }

        public int CompareTo(FiniteDouble other)
        {
            return this._value.CompareTo(other._value);
        }

        public override bool Equals(object obj)
        {
            return obj is FiniteDouble fd &&
                   fd.Value().Equals(this.Value());
        }

        public bool Equals(FiniteDouble other)
        {
            return this.Value().Equals(other.Value());
        }

        public override int GetHashCode()
        {
            return this.Value().GetHashCode();
        }
    }
}
