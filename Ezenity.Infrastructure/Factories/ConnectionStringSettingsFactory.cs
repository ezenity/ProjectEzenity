using Ezenity.Core.Interfaces;
using Ezenity.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;

namespace Ezenity.Infrastructure.Factories
{
    /// <summary>
    /// A factory class responsible for creating instances of IConnectionStringSettings. 
    /// It encapsulates the creation logic and provides a centralized place to manage the instantiation of connection string settings.
    /// </summary>
    public static class ConnectionStringSettingsFactory
    {
        /// <summary>
        /// Creates an IConnectionStringSettings instance based on the application configuration.
        /// It retrieves the 'WebApiDatabase' connection string from the configuration, 
        /// then encapsulates it within a ConnectionStringSettingsWrapper for enhanced abstraction and testability.
        /// </summary>
        /// <param name="configuration">The application configuration which holds the connection strings and other settings.</param>
        /// <returns>An IConnectionStringSettings instance populated with the connection string details from the configuration.</returns>
        public static IConnectionStringSettings Create(IConfiguration configuration)
        {
            // Retrieve the 'WebApiDatabase' connection string from the configuration
            var connectionString = configuration.GetConnectionString("WebApiDatabase");

            // Create a new ConnectionStringSettings instance with the retrieved connection string
            var connectionStringSettings = new ConnectionStringSettings(connectionString);

            // Wrap and return the ConnectionStringSettings instance
            return new ConnectionStringSettingsWrapper(connectionStringSettings);
        }
    }
}
