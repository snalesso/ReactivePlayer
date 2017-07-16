using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Tools.ArtworksExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var libraryDirectoryPath = new Uri(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\library\"));
            var artworksDirectoryPath = Path.Combine(libraryDirectoryPath.LocalPath, @"artworks");

            ExtractArtworks(libraryDirectoryPath.LocalPath, artworksDirectoryPath);

            Console.ReadLine();
        }

        static void ExtractArtworks(string sourceDirectoryPath, string destinationDirectoryPath)
        {
            //var tagsManager = new ReactivePlayer.App.TagLibSharpTrackProfiler();

            //Directory
            //    .GetFiles(sourceDirectoryPath, "*", SearchOption.AllDirectories)
            //    .Select(path => tagsManager.GetTrack(new Uri(path)))
            //    .Where(t => t != null)
            //    .ToList()
            //    .ForEach(track =>
            //    {
            //        var artworks = track.Tags.Artworks.ToList();
            //        artworks.ForEach(artwork =>
            //        {
            //            var artworkPath = Path.Combine(destinationDirectoryPath, $"{Path.GetFileNameWithoutExtension(track.Location.LocalPath)} - {artworks.IndexOf(artwork)}.png");
            //            using (var fs = new FileStream(artworkPath, FileMode.Create))
            //            {
            //                fs.Write(artwork.Data.ToArray(), 0, artwork.Data.Count());
            //            }
            //        });
            //    });
        }
    }
}