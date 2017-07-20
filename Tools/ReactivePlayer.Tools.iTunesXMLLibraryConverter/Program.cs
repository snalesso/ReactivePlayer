using ReactivePlayer.Domain.Repositories;
using System;

namespace ReactivePlayer.Tools.iTunesXMLLibraryConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var xmlItlFilePath = @"..\..\..\..\library\iTunes Music Library.xml";
            var itlRepo = new iTunesTracksRepository(xmlItlFilePath);
            var tracks = itlRepo.GetAllAsync().Result;

            Console.WriteLine("Finished!");
            Console.WriteLine("Press [Enter] to exit ... (HAHAHAH)");
            Console.ReadLine();
        }
    }
}