using Ezenity.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// Contains settings for the applications connection strings
    /// </summary>
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        /// <summary>
        /// Gets the SQL Connection details.
        /// </summary>
        public string WebApiDatabase { get; }

        public ConnectionStringSettings(string webApiDatabase)
        {
            WebApiDatabase = webApiDatabase;
        }
    }
}
