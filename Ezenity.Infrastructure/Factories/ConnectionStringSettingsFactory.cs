using Ezenity.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;

namespace Ezenity.Infrastructure.Factories
{
    /// <summary>
    /// Factory class responsible for creating instances of ConnectionStringSettings.
    /// </summary>
    public static class ConnectionStringSettingsFactory
    {
        /// <summary>
        /// Creates an instance of ConnectionStringSettings based on the provided IConfiguration.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>A populated ConnectionStringSettings object.</returns>
        public static ConnectionStringSettings Create(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("WebApiDatabase");
            return new ConnectionStringSettings(connectionString);
        }
    }
}
