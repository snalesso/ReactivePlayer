using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter.Formatters;

namespace ReactivePlayer.Domain.Repositories
{
    public class ZeroFormatterTracksRepository
    {
        static ZeroFormatterTracksRepository()
        {
            Formatter<DefaultResolver, Uri>.Register(new UriFormatter<DefaultResolver>());
        }
    }
}