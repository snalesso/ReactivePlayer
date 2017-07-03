using ReactivePlayer.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Tools.iTunesXMLLibraryConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var xmlItlFilePath = @"..\..\..\..\library\iTunes Music Library.xml";
            var itlRepo = new iTunesTracksRepository(xmlItlFilePath);
            var tracks = itlRepo.GetTracks().Result;

            Console.WriteLine("Finished!");
            Console.WriteLine("Press [Enter] to exit ... (HAHAHAH)");
            Console.ReadLine();
        }
    }
}