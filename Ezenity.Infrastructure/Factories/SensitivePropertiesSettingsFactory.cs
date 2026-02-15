using Ezenity.Application.Helpers.Exceptions;
using Ezenity.Application.Abstractions.Configuration;
using Ezenity.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;

namespace Ezenity.Infrastructure.Factories
{
    /// <summary>
    /// A factory class for creating instances of ISensitivePropertiesSettings. 
    /// Retrieves the sensitive property settings from the application configuration and encapsulates them in a SensitivePropertiesSettingsWrapper.
    /// </summary>
    public static class SensitivePropertiesSettingsFactory
    {
        /// <summary>
        /// Creates an instance of ISensitivePropertiesSettings based on the provided application configuration.
        /// Retrieves the list of sensitive properties and wraps them for secure and centralized access.
        /// </summary>
        /// <param name="configuration">The application configuration containing the sensitive properties.</param>
        /// <returns>An ISensitivePropertiesSettings implementation with sensitive properties initialized.</returns>
        /// <exception cref="NotFoundException">Thrown if the 'SensitiveErrorProperties' section is missing in the configuration.</exception>
        public static ISensitivePropertiesSettings Create(IConfiguration configuration)
        {
            // Get the sensitive proeprties list from the configuration
            var sensitiveErrorProperties = configuration.GetSection("SensitiveErrorProperties").Get<List<string>>();

            // Ensure senitive properties list from the configuration
            if (sensitiveErrorProperties == null)
            {
                throw new NotFoundException("The 'SensitiveErrorProperties' section is missing in the configuration.");
            }

            // Create an instance of SensitiveProeprtiesConfiguration with the list
            var sensitivePropertiesSettings = new SensitivePropertiesSettings(sensitiveErrorProperties);
            return new SensitivePropertiesSettingsWrapper(sensitivePropertiesSettings);
        }
    }
}
