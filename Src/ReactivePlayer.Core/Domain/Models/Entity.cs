using ReactivePlayer.Core.Domain.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Models
{
    public abstract class Entity : IEquatable<Entity>, INotifyPropertyChanged
    {
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Entity);
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.CombineHashCodes(this.GetIdentityIngredients());
        }

        public bool Equals(Entity other)
        {
            if (other == null)
                return false;

            if (this.GetType() != other.GetType())
                return false;

            return this.GetIdentityIngredients().SequenceEqual(other.GetIdentityIngredients());
        }

        protected abstract IEnumerable<object> GetIdentityIngredients();

        #region operators

        public static bool operator ==(Entity left, Entity right)
        {
            if (left == null ^ right == null)
                return false;

            return (left == null) || left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        #endregion

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        public TProperty SetAndRaiseIfChanged<TProperty>(
            ref TProperty backingField,
            TProperty newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));

            if (EqualityComparer<TProperty>.Default.Equals(backingField, newValue))
                return newValue;

            backingField = newValue;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            return newValue;
        }

        #endregion
    }
}