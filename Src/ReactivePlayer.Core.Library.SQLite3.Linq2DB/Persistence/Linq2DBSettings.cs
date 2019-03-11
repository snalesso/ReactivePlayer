using LinqToDB.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.SQLite3.Linq2DB.Persistence
{
    public class Linq2DBSettings : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders => throw new NotImplementedException();

        public string DefaultConfiguration => throw new NotImplementedException();

        public string DefaultDataProvider => throw new NotImplementedException();

        public IEnumerable<IConnectionStringSettings> ConnectionStrings => throw new NotImplementedException();
    }
}
