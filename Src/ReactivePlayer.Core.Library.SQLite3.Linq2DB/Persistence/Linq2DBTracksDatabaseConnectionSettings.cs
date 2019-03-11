using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.SQLite3.Linq2DB.Persistence
{
    public class Linq2DBTracksDatabaseConnectionSettings : global::LinqToDB.Configuration.IConnectionStringSettings
    {
        public string ConnectionString => throw new NotImplementedException();

        public string Name => "Tracks.Linq2DB";

        public string ProviderName => global::LinqToDB.ProviderName.SQLite;

        public bool IsGlobal => false;
    }
}
