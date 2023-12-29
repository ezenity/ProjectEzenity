using Ezenity.Core.Interfaces;
using Ezenity.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;

namespace Ezenity.Infrastructure.Factories
{
    public static class ConnectionStringSettingsFactory
    {
        public static IConnectionStringSettings Create(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("WebApiDatabase");
            var connectionStringSettings = new ConnectionStringSettings(connectionString);
            return new ConnectionStringSettingsWrapper(connectionStringSettings);
        }
    }
}
