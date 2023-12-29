using Ezenity.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Helpers
{
    public class SensitivePropertiesSettingsWrapper : ISensitivePropertiesSettings
    {
        private readonly ISensitivePropertiesSettings _innerSettings;

        public SensitivePropertiesSettingsWrapper(ISensitivePropertiesSettings innerSettings)
        {
            _innerSettings = innerSettings ?? throw new ArgumentNullException(nameof(innerSettings));
        }

        public List<string> SensitiveErrorProperties => _innerSettings.SensitiveErrorProperties;
    }
}
