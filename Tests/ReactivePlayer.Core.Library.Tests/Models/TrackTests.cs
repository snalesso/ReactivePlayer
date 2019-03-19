using ReactivePlayer.Core.Library.Models;
using System;
using System.Globalization;
using Xunit;

namespace ReactivePlayer.Core.Library.Tests.Models
{
    public class TrackTests
    {
        private const string dtf = "dd/MM/yyyy HH:mm";
        private static readonly IFormatProvider dtFormatProvider = CultureInfo.InvariantCulture;
        private static readonly Track LinkinPark_Iridescent = new Track(
            1,
            new Uri(@"D:\Music\Linkin Park - Iridescent.mp3"),
            new TimeSpan(0, 4, 57),
            DateTime.ParseExact("21/12/2016 19:10", dtf, dtFormatProvider),
            11_900_450U,
            DateTime.ParseExact("21/08/2012 20:32", dtf, dtFormatProvider),
            true,
            "Iridescent",
            new[]
            {
                new Artist("Linkin Park")
            },
            new[]
            {
                new Artist("Linkin Park")
            },
            2010,
            new TrackAlbumAssociation(
                new Album(
                    "A Thousand Suns",
                    new[]
                    {
                        new Artist("Linkin Park")
                    },
                    15,
                    1),
                12,
                1));
        private static readonly Track LinkinPark_TheMessenger = new Track(
            2,
            new Uri(@"D:\Music\Linkin Park - The messenger.mp3"),
            new TimeSpan(0, 3, 2),
            DateTime.ParseExact("21/12/2016 19:11", dtf, dtFormatProvider),
            7_314_169U,
            DateTime.ParseExact("21/08/2012 20:31", dtf, dtFormatProvider),
            true,
            "The messenger",
            new[]
            {
                new Artist("Linkin Park")
            },
            new[]
            {
                new Artist("Linkin Park")
            },
            2010,
            new TrackAlbumAssociation(
                new Album(
                    "A Thousand Suns",
                    new[]
                    {
                        new Artist("Linkin Park")
                    },
                    15,
                    1),
                15,
                1));

        [Fact]
        public void A_first_gen_entity_is_equal_to_itself()
        {
            Assert.True(LinkinPark_Iridescent.Equals(LinkinPark_Iridescent));
        }

        [Fact]
        public void Two_first_gen_entities_with_same_id_are_equal()
        {
            Assert.True(LinkinPark_Iridescent.Equals(LinkinPark_TheMessenger));
            Assert.True(LinkinPark_Iridescent == LinkinPark_TheMessenger);
        }

        //[Fact]
        //public void Two_first_gen_entities_different_only_by_id_are_different()
        //{
        //    var p1 = LinkinPark_Iridescent;

        //    var p2Id = Guid.NewGuid();
        //    while (p2Id == p1.Id)
        //    {
        //        p2Id = Guid.NewGuid();
        //    }
        //    var p2 = LinkinPark_TheMessenger;

        //    Assert.False(p1.Equals(p2));
        //    Assert.False(p1 == p2);
        //}

        //[Fact]
        //public void First_and_second_gen_entities_with_equal_common_props_are_equal()
        //{
        //    var firstGen = new Track(Guid.NewGuid(), "Gino", "Panino", DateTime.Now.Add(new TimeSpan(365 * 20, 43, 35, 02, 83).Negate()));
        //    var secondGen = new Employee(firstGen.Id, firstGen.FirstName, firstGen.LastName, firstGen.BirthDateTime, "fejwofjew");

        //    Assert.False(firstGen.Equals(secondGen));
        //    Assert.False(firstGen == secondGen);
        //}
    }
}
