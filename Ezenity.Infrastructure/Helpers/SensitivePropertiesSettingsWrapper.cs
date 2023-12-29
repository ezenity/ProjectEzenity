using Ezenity.Core.Interfaces;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// A wrapper for the ISensitivePropertiesSettings interface. 
    /// Provides an abstraction layer over the sensitive properties settings, allowing for future extensibility and easier testing.
    /// </summary>
    public class SensitivePropertiesSettingsWrapper : ISensitivePropertiesSettings
    {
        private readonly ISensitivePropertiesSettings _innerSettings;

        /// <summary>
        /// Initializes a new instance of the SensitivePropertiesSettingsWrapper class with the specified settings.
        /// </summary>
        /// <param name="innerSettings">The actual implementation of ISensitivePropertiesSettings to wrap.</param>
        public SensitivePropertiesSettingsWrapper(ISensitivePropertiesSettings innerSettings)
        {
            _innerSettings = innerSettings ?? throw new ArgumentNullException(nameof(innerSettings));
        }

        /// <summary>
        /// Gets the list of property names that are considered sensitive.
        /// </summary>
        public List<string> SensitiveErrorProperties => _innerSettings.SensitiveErrorProperties;
    }
}
