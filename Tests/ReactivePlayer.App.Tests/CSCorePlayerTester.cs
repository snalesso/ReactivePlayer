using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReactivePlayer.Core.Tests
{
    public class CSCorePlayerTester
    {
        private const string LibraryDirectoryRelativePath = @"..\..\..\..\library\";

        //[Fact]
        //public void PlayerDefaultStatusNone()
        //{
        //    var player = new CSCorePlayer();
        //}

        //[Fact]
        //public void Play()
        //{
        //    var player = new CSCorePlayer();
        //    var libraryDirectoryLocation = new Uri(Path.Combine(Environment.CurrentDirectory, LibraryDirectoryRelativePath)).AbsolutePath;
        //    var tracksPaths = Directory.GetFiles(LibraryDirectoryRelativePath, "*", SearchOption.TopDirectoryOnly).Where(p => player.SupportedExtensions.Contains(Path.GetExtension(p))).ToArray();
        //    var r = new Random((int)DateTime.Now.Ticks);
        //    var randomTrackPath = tracksPaths.ElementAt(r.Next(0, tracksPaths.Count()));
            
        //    player.PlayAsync(new Uri(randomTrackPath)).Wait();
        //}
    }
}