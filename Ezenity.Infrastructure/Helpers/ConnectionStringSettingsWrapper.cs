using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Helpers
{
    public class ConnectionStringSettingsWrapper
    {
        private readonly ConnectionStringSettings _connectionString;

        public ConnectionStringSettingsWrapper(ConnectionStringSettings connectionString)
        {
            _connectionString = connectionString;
        }

        public string WebApiDatabase => _connectionString.WebApiDatabase;
    }
}
