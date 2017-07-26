using Akavache;
using Akavache.Sqlite3;
using System.Reactive.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Entities;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Disposables;

namespace ReactivePlayer.Domain.Repositories
{
    public class AkavacheArtworksRepository 
    {
        public AkavacheArtworksRepository()
        {
            var x = new Akavache.Sqlite3.SQLitePersistentBlobCache("");
            Akavache.BlobCache.ApplicationName = nameof(ReactivePlayer);
            Akavache.BlobCache.LocalMachine = new Akavache.Sqlite3.SQLitePersistentBlobCache("");
        }        
    }
}