using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Common.Extensions
{
    public static class UriExtensions
    {
        public static bool IsLocalFileAndExists(this Uri uri)
        {
            return uri.IsFile && System.IO.File.Exists(uri.LocalPath);
        }
    }
}