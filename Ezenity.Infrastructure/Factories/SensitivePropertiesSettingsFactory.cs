using Ezenity.Core.Helpers.Exceptions;
using Ezenity.Core.Interfaces;
using Ezenity.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Ezenity.Infrastructure.Factories
{
    public static class SensitivePropertiesSettingsFactory
    {
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
