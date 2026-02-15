using Ezenity.Application.Abstractions.Configuration;
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
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            //Console.WriteLine($"Factory Class | Environment: {environment}");
            //Console.WriteLine($"Factory Class | DB Name: {Environment.GetEnvironmentVariable("EZENITY_DATABASE_NAME")}");
            //Console.WriteLine($"Factory Class | DB User: {Environment.GetEnvironmentVariable("EZENITY_DATABASE_USER")}");

            // 1) Prefer standard .NET connection strings (supports env: ConnectionStrings__WebApiDatabase)
            var direct =
                configuration.GetConnectionString("WebApiDatabase")
                ?? configuration["ConnectionStrings:WebApiDatabase"];

            if (!string.IsNullOrWhiteSpace(direct))
            {
                Console.WriteLine("Factory Class | Using ConnectionStrings:WebApiDatabase");
                return new ConnectionStringSettingsWrapper(new ConnectionStringSettings(direct));
            }

            // 2) Fallback: build from your custom env vars (optional)
            var dbName = Environment.GetEnvironmentVariable("EZENITY_DATABASE_NAME");
            var dbUser = Environment.GetEnvironmentVariable("EZENITY_DATABASE_USER");
            var dbPassword = Environment.GetEnvironmentVariable("EZENITY_DATABASE_PASSWORD");

            //Console.WriteLine($"Factory Class | DB Password: {(string.IsNullOrEmpty(dbPassword) ? "(empty)" : "(set)")}");

            // We MUST NOT use localhost in containers unless DB runs in same container.
            // If your DB runs on the host machine, use host.docker.internal (with extra_hosts mapping).
            var dbHost = Environment.GetEnvironmentVariable("EZENITY_DATABASE_HOST") ?? "host.docker.internal";
            var dbPort = Environment.GetEnvironmentVariable("EZENITY_DATABASE_PORT") ?? "3306";

            if (string.IsNullOrWhiteSpace(dbName) ||
                string.IsNullOrWhiteSpace(dbUser) ||
                string.IsNullOrWhiteSpace(dbPassword))
            {
                throw new InvalidOperationException(
                    "Database connection is not configured. Set ConnectionStrings__WebApiDatabase (recommended) " +
                    "or set EZENITY_DATABASE_NAME / EZENITY_DATABASE_USER / EZENITY_DATABASE_PASSWORD (fallback).");
            }

            //Console.WriteLine($"Factory Class | Using built DB config (Host={dbHost}, Port={dbPort}, Db={dbName}, User={dbUser})");

            var built = $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};";
            return new ConnectionStringSettingsWrapper(new ConnectionStringSettings(built));
        }
    }
}
