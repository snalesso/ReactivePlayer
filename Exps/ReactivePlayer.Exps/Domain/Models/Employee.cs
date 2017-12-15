using Daedalus.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Exps.Domain.Models
{
    public class Employee : Person
    {
        public Employee(
            Guid id,
            string firstName,
            string lastName,
            DateTime birthDateTime,
            string companyName)
            : base(id, firstName, lastName, birthDateTime)
        {
            this.CompanyName = companyName.TrimmedOrNull() ?? throw new ArgumentNullException(nameof(companyName));
        }

        public string CompanyName { get; }
    }
}
