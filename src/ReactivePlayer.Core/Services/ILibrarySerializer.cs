using ReactivePlayer.Core.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data.Services
{
    public interface ILibrarySerializer
    {
        bool Serialize(ILibrary library, string libraryPath);
        ILibrary Deserialize(string libraryPath);
    }
}