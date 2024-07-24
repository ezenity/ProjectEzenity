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
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isProduction = environment == "Production";

            string connectionString;

            Console.WriteLine($"Factory Class | Environemnt: {environment}");
            
            Console.WriteLine($"Factory Class | Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
            Console.WriteLine($"Factory Class | DB Name: {Environment.GetEnvironmentVariable("EZENITY_DATABASE_NAME")}");
            Console.WriteLine($"Factory Class | DB User: {Environment.GetEnvironmentVariable("EZENITY_DATABASE_USER")}");
            Console.WriteLine($"Factory Class | DB Password: {Environment.GetEnvironmentVariable("EZENITY_DATABASE_PASSWORD")}");


            if (isProduction)
            {
                // Construct the connection string using environment variables for Production
                var dbName = Environment.GetEnvironmentVariable("EZENITY_DATABASE_NAME") ?? throw new InvalidOperationException("Database name must be provided in environment variables for Production.");
                var dbUser = Environment.GetEnvironmentVariable("EZENITY_DATABASE_USER") ?? throw new InvalidOperationException("Database user must be provided in environment variables for Production.");
                var dbPassword = Environment.GetEnvironmentVariable("EZENITY_DATABASE_PASSWORD") ?? throw new InvalidOperationException("Database password must be provided in environment variables for Production.");

                connectionString = $"Server=localhost;Database={dbName};User={dbUser};Password={dbPassword};";
            }
            else
            {
                // Use the connection string from configuration settings for Development
                connectionString = configuration.GetConnectionString("WebApiDatabase") ?? throw new InvalidOperationException("Connection string 'WebApiDatabase' must be defined in configuration for Development.");
            }

            Console.WriteLine($"Factory Class | Environemnt: {connectionString}");

            // Create a new ConnectionStringSettings instance with the retrieved connection string
            var connectionStringSettings = new ConnectionStringSettings(connectionString);

            // Wrap and return the ConnectionStringSettings instance
            return new ConnectionStringSettingsWrapper(connectionStringSettings);
        }
    }
}
