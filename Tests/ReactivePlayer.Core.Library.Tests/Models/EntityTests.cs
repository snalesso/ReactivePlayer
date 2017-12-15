using ReactivePlayer.Exps.Domain.Models;
using System;
using Xunit;

namespace ReactivePlayer.Core.Library.Tests.Models
{
    public class EntityTests
    {
        [Fact]
        public void A_first_gen_entity_is_equal_to_itself()
        {
            var p = new Person(Guid.NewGuid(), "Gino", "Panino", DateTime.Now.Add(new TimeSpan(365 * 20, 43, 35, 02, 83).Negate()));

            Assert.True(p.Equals(p));
        }

        [Fact]
        public void Two_first_gen_entities_with_same_id_are_equal()
        {
            var id = Guid.Parse("{9EE4EDD0-51D8-4109-A4BD-C938C3B7C21A}");
            var p1 = new Person(id, "Gino", "Panino", DateTime.Now.Add(new TimeSpan(365 * 20, 43, 35, 02, 83).Negate()));
            var p2 = new Person(id, "Gaia", "Zoccola", DateTime.Now.Add(new TimeSpan(365 * 23, 378, 375, 2896, 57).Negate()));

            Assert.True(p1.Equals(p2));
            Assert.True(p1 == p2);
        }

        [Fact]
        public void Two_first_gen_entities_different_only_by_id_are_different()
        {
            var p1 = new Person(Guid.NewGuid(), "Gino", "Panino", DateTime.Now.Add(new TimeSpan(365 * 20, 43, 35, 02, 83).Negate()));

            var p2Id = Guid.NewGuid();
            while (p2Id == p1.Id)
            {
                p2Id = Guid.NewGuid();
            }
            var p2 = new Person(p2Id, p1.FirstName, p1.LastName, p1.BirthDateTime);

            Assert.False(p1.Equals(p2));
            Assert.False(p1 == p2);
        }

        [Fact]
        public void First_and_second_gen_entities_with_equal_common_props_are_equal()
        {
            var firstGen = new Person(Guid.NewGuid(), "Gino", "Panino", DateTime.Now.Add(new TimeSpan(365 * 20, 43, 35, 02, 83).Negate()));
            var secondGen = new Employee(firstGen.Id, firstGen.FirstName, firstGen.LastName, firstGen.BirthDateTime, "fejwofjew");

            Assert.False(firstGen.Equals(secondGen));
            Assert.False(firstGen == secondGen);
        }
    }
}
