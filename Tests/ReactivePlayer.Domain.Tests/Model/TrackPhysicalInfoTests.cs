using ReactivePlayer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReactivePlayer.Domain.Tests.Model
{
    public sealed class TrackPhysicalInfoTests
    {
        [Fact]
        public void A_TrackPhysicalInfo_is_equal_to_itself()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var duration = TimeSpan.FromMilliseconds(5 * 60 * 1000 + 3 * 1000 + 197); // 00:05:03.197
            var lastModifiedDateTime = DateTime.Now.Add(duration.Negate());
            var tpi = new TrackFileInfo(new Uri(location), duration, lastModifiedDateTime);

            Assert.True(tpi.Equals(tpi));
        }

        [Theory]
        [InlineData(@"C:\folder\Another Folder\File name.ext", -1, null)]
        [InlineData(@"C:\folder\Another Folder\File name.ext", 0, null)]
        [InlineData(@"C:\folder\Another Folder\File name.ext", 47984351439, null)]
        [InlineData(@"C:\folder\Another Folder\File name.ext", -1, "2017/07/09 16:11:07")]
        [InlineData(@"C:\folder\Another Folder\File name.ext", 0, "2017/07/09 16:11:07")]
        [InlineData(@"C:\folder\Another Folder\File name.ext", 47984351439, "2017/07/09 16:11:07")]
        public void TrackPhysicalInfo_with_same_values_are_equal(string location, long durationTicks, string lmdtString)
        {
            var duration1 = durationTicks >= 0 ? TimeSpan.FromTicks(durationTicks) : (null as TimeSpan?);
            var duration2 = durationTicks >= 0 ? TimeSpan.FromTicks(durationTicks) : (null as TimeSpan?);

            var lmdt1 =
                !string.IsNullOrEmpty(lmdtString)
                ? duration1.HasValue ? DateTime.Parse(lmdtString).Add(duration1.Value.Negate()) : DateTime.Parse(lmdtString)
                : (null as DateTime?);
            var lmdt2 =
                !string.IsNullOrEmpty(lmdtString)
                ? duration2.HasValue ? DateTime.Parse(lmdtString).Add(duration2.Value.Negate()) : DateTime.Parse(lmdtString)
                : (null as DateTime?);

            var tpi1 = new TrackFileInfo(new Uri(location), duration1, lmdt1);
            var tpi2 = new TrackFileInfo(new Uri(location), duration2, lmdt2);

            Assert.True(tpi1 == tpi2);
        }

        [Fact]
        public void TrackPhysicalInfo_null_location_throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TrackFileInfo(null, null, null));
        }
    }
}