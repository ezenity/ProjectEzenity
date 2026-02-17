using Ezenity.Application.Abstractions.Configuration;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// contains a list of property names considered sensitive and should be excludedfrom error metadata.
    /// </summary>
    public class SensitivePropertiesSettings : ISensitivePropertiesSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensitivePropertiesSettings"/> class.
        /// </summary>
        /// <param name="sensitiveErrorProperties">A list of property names to be considered sensitive.</param>
        public SensitivePropertiesSettings(List<string> sensitiveErrorProperties)
        {
            SensitiveErrorProperties = sensitiveErrorProperties ?? new List<string>();
        }

        /// <summary>
        /// Gets the list of proeprty names considered senitive
        /// </summary>
        public List<string> SensitiveErrorProperties { get; }
    }
}
