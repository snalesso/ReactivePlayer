using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Exps.Domain.Models
{
    public class Person : Entity<Guid>//, IEquatable<Person>
    {
        #region ctor

        public Person(
            Guid id,
            string firstName,
            string lastName,
            DateTime birthDateTime)
            : base(id)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.BirthDateTime = birthDateTime;
        }

        #endregion

        #region properties

        public string FirstName { get; }

        public string LastName { get; }

        public DateTime BirthDateTime { get; }

        #endregion
    }
}