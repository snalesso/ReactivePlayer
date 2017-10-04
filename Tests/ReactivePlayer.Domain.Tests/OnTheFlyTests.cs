using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReactivePlayer.Domain.Tests
{
    public sealed class OnTheFlyTests
    {
        //[Theory]
        //// no special chars
        //[InlineData(@"C:\a\folder 2\file_name.txt", @"C:\a\folder 2\file_name.txt")] // same
        //[InlineData(@"C:\a\FOLDER 2\file_name.txt", @"C:\a\folder 2\file_name.txt")] // folder upper
        ////[InlineData(@"C:\a\folder 2\file_name.txt", @"C:\a\folder 2\FILE_NAME.TXT")] // file upper
        //// special chars
        //[InlineData(@"C:\a\fòlder 2\file_nàme.txt", @"C:\a\fòlder 2\file_nàme.txt")] // same
        //[InlineData(@"C:\a\FÒLDER 2\file_nàme.txt", @"C:\a\fòlder 2\file_nàme.txt")] // folder upper
        ////[InlineData(@"C:\a\fòlder 2\file_nàme.txt", @"C:\a\fòlder 2\FILE_NÀME.TXT")] // file upper
        ////// URLs
        ////[InlineData(@"https://www.api.spotify.com/track/aaaaaaaa0aNN92jL", @"https://www.api.spoTIFY.COM/TRAck/aaaaaaaa0aNN92jL")] // different directory case
        ////[InlineData(@"https://www.api.spotify.com/track/aaaaaaaa0aNN92jL", @"https://www.api.spotify.com/track/aaaAAaaa0aNN92jL")] // different IDs case
        //public void Uri_file_names_are_equal(string firstUriSource, string secondUriSource)
        //{
        //    var u1 = new Uri(firstUriSource);
        //    var u2 = new Uri(secondUriSource);

        //    Func<Uri, string> service = (Uri u) => System.IO.Path.GetDirectoryName(u.AbsoluteUri);
        //    Func<Uri, string> fileId = (Uri u) => System.IO.Path.GetFileName(u.AbsoluteUri);

        //    var u1Pieces = new[] { service(u1), fileId(u1) };
        //    var u2Pieces = new[] { service(u2), fileId(u2) };

        //    var areServicesEqual = u1Pieces[0].Equals(u2Pieces[0], StringComparison.OrdinalIgnoreCase);
        //    var areIdsEqual = u1Pieces[1].Equals(u2Pieces[1], StringComparison.Ordinal);

        //    Assert.True(areServicesEqual);
        //    Assert.True(areIdsEqual);
        //}

        [Fact]
        public void Nullable_structures_are_equatable()
        {
            var dtString = DateTime.Now.ToString();
            DateTime? dt_n0 = null;
            DateTime? dt_n1 = DateTime.Parse(dtString);
            DateTime? dt_n2 = DateTime.Parse(dtString);

            Assert.True(dt_n0 == null);
            Assert.True(dt_n1 == dt_n2);
        }
    }
}