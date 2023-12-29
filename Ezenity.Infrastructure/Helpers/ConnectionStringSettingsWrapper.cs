using Ezenity.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Helpers
{
    public class ConnectionStringSettingsWrapper : IConnectionStringSettings
    {
        private readonly IConnectionStringSettings _connectionString;

        public ConnectionStringSettingsWrapper(IConnectionStringSettings connectionStringSettings)
        {
            _connectionString = connectionStringSettings ?? throw new ArgumentNullException(nameof(connectionStringSettings));
        }

        public string WebApiDatabase => _connectionString.WebApiDatabase;
    }
}
