using System;

namespace WaveformTimeline.Primitives
{
    /// <summary>
    /// An integer that must be even
    /// </summary>
    public struct Even : IComparable<Even>, IEquatable<Even>
    {
        public Even(int value) : this(value, 1) { }

        public Even(int value, int offset)
        {
            this._value = value;
            this._offset = Math.Abs(offset) == 1
                ? offset
                : 1;
        }

        private readonly int _value;
        private readonly int _offset;

        public int Value()
        {
            return
                this._value % 2 == 0
                ? this._value
                : this._value + this._offset;
        }

        public int CompareTo(Even other)
        {
            return this.Value().CompareTo(other.Value());
        }

        public bool Equals(Even other)
        {
            return this.Value().Equals(other.Value());
        }

        public override bool Equals(object obj)
        {
            return obj is Even even && this.Value().Equals(even.Value());
        }

        public override int GetHashCode()
        {
            return this.Value().GetHashCode();
        }

        public static bool operator ==(Even one, Even two)
        {
            return one.Value().Equals(two.Value());
        }

        public static bool operator !=(Even one, Even two)
        {
            return !(one == two);
        }

        public static implicit operator int(Even even)
        {
            return even.Value();
        }

        public static implicit operator Even(int value)
        {
            return new Even(value);
        }

        public override string ToString()
        {
            return this.Value().ToString();
        }
    }
}