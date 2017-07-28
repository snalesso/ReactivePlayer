﻿using ReactivePlayer.Domain.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Models
{
    public abstract class Entity : IEquatable<Entity>
    {
        protected abstract bool EqualsCore(Entity other);
        public bool Equals(Entity other)
        {
            return
              other != null
              && this.GetType() == other.GetType() // TODO: does this work if I pass an inheriting class instance??
              && this.EqualsCore(other);
        }
        public override bool Equals(object obj) => this.Equals(obj as Entity);

        protected abstract IEnumerable<object> GetHashCodeComponents();
        public override int GetHashCode() => HashCodeHelper.CombineHashCodes(this.GetHashCodeComponents());

        public static bool operator ==(Entity left, Entity right)
        {
            if (object.ReferenceEquals(left, null) ^ object.ReferenceEquals(right, null))
                return false;

            return object.ReferenceEquals(left, null) || left.Equals(right);
        }
        public static bool operator !=(Entity left, Entity right) =>
            !(left == right);
    }

    //public class Artistt : Entityy
    //{
    //    public Artistt(string name)
    //    {
    //        this.Name = name.TrimmedOrNull() ?? throw new ArgumentNullException();
    //    }

    //    public string Name { get; private set; }

    //    protected override IEnumerable<object> GetNativeIdentityComponents()
    //    {
    //        yield return this.Name;
    //    }
    //}

    //public class Trackk : EntityyWithArtificialId<Guid>
    //{
    //    public Trackk(Guid id, string path) : base(id)
    //    {
    //    }

    //    public string Path { get; private set; }

    //    protected override bool IsWellFormedId(Guid id)
    //    {
    //        return id != null && id != Guid.Empty;
    //    }
    //}
}