using Ezenity.Application.Abstractions.Configuration;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// Wraps the IConnectionStringSettings interface, providing a layer of abstraction over the connection string settings.
    /// This allows for future enhancements and facilitates easier unit testing by enabling mocking of connection string values.
    /// </summary>
    public class ConnectionStringSettingsWrapper : IConnectionStringSettings
    {
        private readonly IConnectionStringSettings _connectionString;

        /// <summary>
        /// Initializes a new instance of the ConnectionStringSettingsWrapper class with the specified IConnectionStringSettings implementation.
        /// </summary>
        /// <param name="connectionStringSettings">The actual implementation of IConnectionStringSettings to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided connectionStringSettings is null.</exception>
        public ConnectionStringSettingsWrapper(IConnectionStringSettings connectionStringSettings)
        {
            _connectionString = connectionStringSettings ?? throw new ArgumentNullException(nameof(connectionStringSettings));
        }

        /// <summary>
        /// Gets the database connection string for the Web API from the wrapped IConnectionStringSettings instance.
        /// </summary>
        public string WebApiDatabase => _connectionString.WebApiDatabase;
    }
}
