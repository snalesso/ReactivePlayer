using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data.IO
{
    public interface IFileSystemService
    {
        Task MoveFileToRecycleBinAsync(string filePath);
        Task LockFile(string path);
        Task UnlockFile(string path);
    }
}