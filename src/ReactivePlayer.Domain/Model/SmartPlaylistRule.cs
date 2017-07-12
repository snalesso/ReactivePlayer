using Daedalus.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Model
{
    public enum SmartPlaylistRulesJoinMode
    {
        Any,
        All
    }

    public enum StringCasingSensitivity
    {
        Sensitive,
        Insensitive
    }

    public enum StringContentPredicate
    {
        EqualsTo,
        Contains,
        StartsWith,
        EndWith
    }

    public enum NumberPredicate
    {
        IsLowerThan,
        EqualsTo,
        IsGreaterThan,

        IsLowerThanOrEqualTo = IsLowerThan | EqualsTo,
        IsGreaterThanOrEqualTo = IsGreaterThan | EqualsTo
    }

    public enum SmartPlaylistRuleSatisfactionMode
    {
        Positively,
        Negatively
    }

    public abstract class SmartPlaylistRule<T> : ValueObject<SmartPlaylistRule<T>>
    {
        public SmartPlaylistRule(
            string propertyName,
            T value,
            SmartPlaylistRuleSatisfactionMode satisfactionMode,
            bool isNullAccepted)
        {
            /* TODO: more accurate exception
             * TODO: localize */
            this.Property = typeof(Track).GetProperty(propertyName) ?? throw new ArgumentOutOfRangeException(nameof(propertyName));
            this.Value = value;
            this.SatisfactionMode = satisfactionMode;
            this.IsNullAccepted = isNullAccepted;
        }

        public PropertyInfo Property { get; }
        public T Value { get; }
        public SmartPlaylistRuleSatisfactionMode SatisfactionMode { get; }
        public bool IsNullAccepted { get; }

        protected T GetPropertyValue(Track track) => (T)this.Property.GetValue(track);

        protected Expression<Func<Track, bool>> _predicate;
        //public bool IsSatisfied(Track track) => this._predicate?. ?? false;
    }

    public class SmartPlaylistRule_String : SmartPlaylistRule<string>
    {
        public SmartPlaylistRule_String(
            string propertyName,
            string value,
            SmartPlaylistRuleSatisfactionMode satisfactionMode,
            bool isNullAccepted)
            : base(
                  propertyName,
                  value.TrimmedOrNull() ?? throw new ArgumentNullException(nameof(value)),
                  satisfactionMode,
                  isNullAccepted)
        {
        }

        public override bool Equals(SmartPlaylistRule<string> other)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            throw new NotImplementedException();
        }
    }

    public class SmartPlaylistRule_Number : SmartPlaylistRule<string>
    {
        public SmartPlaylistRule_Number(
            string propertyName,
            string value,
            SmartPlaylistRuleSatisfactionMode satisfactionMode,
            bool isNullAccepted)
            : base(
                  propertyName,
                  value,
                  satisfactionMode,
                  isNullAccepted)
        {
        }

        public override bool Equals(SmartPlaylistRule<string> other)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            throw new NotImplementedException();
        }
    }
}