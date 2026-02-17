using Ezenity.Application.Abstractions.Configuration;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// Represents the connection string settings used within the application. 
    /// Implements the IConnectionStringSettings interface to provide a standardized way to access connection strings.
    /// </summary>
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        /// <summary>
        /// Initializes a new instance of the ConnectionStringSettings class with the specified database connection string.
        /// </summary>
        /// <param name="webApiDatabase">The connection string for the Web API database.</param>
        public ConnectionStringSettings(string webApiDatabase)
        {
            WebApiDatabase = webApiDatabase ?? throw new ArgumentNullException(nameof(webApiDatabase), "Database connection string cannot be null.");
        }

        /// <summary>
        /// Gets the connection string for the Web API database.
        /// </summary>
        public string WebApiDatabase { get; }
    }
}
