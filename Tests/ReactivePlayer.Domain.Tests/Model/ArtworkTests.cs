using ReactivePlayer.Core.Library.Models;
using System;
using System.Linq;
using Xunit;

namespace ReactivePlayer.Domain.Tests.Model
{
    public sealed class ArtworkTests
    {
        [Fact]
        public void Different_instances_of_IEnumerable_of_bytes_containing_same_bytes_are_NOT_equal()
        {
            const int N = 1000;
            var r = new Random((int)DateTime.Now.Ticks);
            var data1 = new byte[N];
            r.NextBytes(data1);
            var data2 = new byte[N];
            data1.CopyTo(data2, 0);

            Assert.False(data1.Equals(data2));
        }

        [Fact]
        public void An_artwork_is_equal_to_itself()
        {
            const int N = 1000;
            var r = new Random((int)DateTime.Now.Ticks);
            var data1 = new byte[N];
            r.NextBytes(data1);
            var xxHash = new System.Data.HashFunction.xxHash(64);
            var data1Hash = System.Text.Encoding.ASCII.GetString(xxHash.ComputeHash(data1));

            var a1 = new Artwork(new ArtworkData(data1Hash, data1), ArtworkType.Cover);

            Assert.True(a1.Equals(a1));
        }

        [Fact]
        public void Different_byte_arrays_with_same_values_are_equal()
        {
            const int N = 1000;
            var r = new Random((int)DateTime.Now.Ticks);
            var data1 = new byte[N];
            r.NextBytes(data1);
            var data2 = new byte[N];
            data1.CopyTo(data2, 0);

            Assert.True(data1.SequenceEqual(data2));
        }

        [Fact]
        public void Artworks_with_same_attributes_are_equal()
        {
            const int N = 5;
            var r = new Random((int)DateTime.Now.Ticks);
            var xxHash = new System.Data.HashFunction.xxHash(64);

            var data1 = new byte[N];
            r.NextBytes(data1);
            var data2 = new byte[N];
            data1.CopyTo(data2, 0);
            var data1Hash = System.Text.Encoding.ASCII.GetString(xxHash.ComputeHash(data1));
            var data2Hash = System.Text.Encoding.ASCII.GetString(xxHash.ComputeHash(data2));

            var a1 = new Artwork(new ArtworkData(data1Hash, data1), ArtworkType.Cover);
            var a2 = new Artwork(new ArtworkData(data2Hash, data2), ArtworkType.Cover);

            Assert.True(a1 == a2);
        }

        [Fact]
        public void Artworks_different_only_by_data_are_NOT_equal()
        {
            const int N = 1000;
            var r = new Random((int)DateTime.Now.Ticks);
            var xxHash = new System.Data.HashFunction.xxHash(64);

            var data1 = new byte[N];
            r.NextBytes(data1);
            var data2 = new byte[N];
            data1.CopyTo(data2, 0);
            var i = r.Next(0, N);
            var xi = (data2[i] + 1) % 256;
            data2[i] = (byte)xi;
            var data1Hash = System.Text.Encoding.ASCII.GetString(xxHash.ComputeHash(data1));
            var data2Hash = System.Text.Encoding.ASCII.GetString(xxHash.ComputeHash(data2));

            var a1 = new Artwork(new ArtworkData(data1Hash, data1), ArtworkType.Cover);
            var a2 = new Artwork(new ArtworkData(data2Hash, data2), ArtworkType.Cover);

            Assert.True(a1 != a2);
        }

        //[Theory]
        //[InlineData(ImageMimeType.Bmp, ImageMimeType.Exif)]
        //[InlineData(ImageMimeType.Bmp, ImageMimeType.Gif)]
        //[InlineData(ImageMimeType.Bmp, ImageMimeType.Ico)]
        //[InlineData(ImageMimeType.Bmp, ImageMimeType.Jpeg)]
        //[InlineData(ImageMimeType.Bmp, ImageMimeType.Png)]
        //[InlineData(ImageMimeType.Bmp, ImageMimeType.Tiff)]
        //public void Artworks_different_only_by_MimeType_are_NOT_equal(ImageMimeType mt1, ImageMimeType mt2)
        //{
        //    const int N = 1000;
        //    var r = new Random((int)DateTime.Now.Ticks);
        //    var xxHash = new System.Data.HashFunction.xxHash(64);

        //    var data1 = new byte[N];
        //    r.NextBytes(data1);
        //    var data1Hash = System.Text.Encoding.ASCII.GetString(xxHash.ComputeHash(data1));
        //    var data2Hash = System.Text.Encoding.ASCII.GetString(xxHash.ComputeHash(data1));

        //    var a1 = new Artwork(new ArtworkData(data1Hash, data1), ArtworkType.Cover);
        //    var a2 = new Artwork(new ArtworkData(data2Hash, data2), ArtworkType.Cover);

        //    Assert.True(a1 != a2);
        //}

        [Fact]
        public void Artworks_different_only_by_type_are_NOT_equal()
        {
            const int N = 1000;
            var r = new Random((int)DateTime.Now.Ticks);
            var xxHash = new System.Data.HashFunction.xxHash(64);

            var data1 = new byte[N];
            var data1Hash = System.Text.Encoding.ASCII.GetString(xxHash.ComputeHash(data1));

            var a1 = new Artwork(new ArtworkData(data1Hash, data1), ArtworkType.Cover);
            var a2 = new Artwork(new ArtworkData(data1Hash, data1), ArtworkType.Back);

            Assert.True(a1 != a2);
        }
    }
}