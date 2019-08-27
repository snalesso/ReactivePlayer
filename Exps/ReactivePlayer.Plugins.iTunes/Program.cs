using ReactivePlayer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.iTunes
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new iTunesXMLPlaylistsDeserializer();
            x.GetAllAsync();

            Console.ReadLine();
        }
    }
}
