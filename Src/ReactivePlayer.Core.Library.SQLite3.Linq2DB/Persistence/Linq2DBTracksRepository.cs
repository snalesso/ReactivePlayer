using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.SQLite3.Linq2DB.Persistence
{
    public sealed class Linq2DBTracksRepository : DataConnection
    {
        public Linq2DBTracksRepository(string dbFilePath)
            : base(
                  new LinqToDB.DataProvider.SQLite.SQLiteDataProvider()
                  , dbFilePath
                  //, new LinqToDB.Mapping.MappingSchema()
                  )
        {
        }
    }
}
