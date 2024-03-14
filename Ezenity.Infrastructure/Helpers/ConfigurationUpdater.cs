using AutoMapper;
using Ezenity.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Helpers
{
    public class ConfigurationUpdater : IConfigurationUpdater
    {
        public IConfigurationRoot UpdateConfiguration(IConfiguration configuration)
        {
            var configurationRoot = configuration as IConfigurationRoot;
            var configurationProvider = new EnvironmentVariablesConfigurationProvider();

            foreach (var child in configuration.GetChildren())
            {
                ReplacePlaceHolderWithEnvironmentVariable(child, configurationProvider);
            }

            return configurationRoot;
        }

        private void ReplacePlaceHolderWithEnvironmentVariable(IConfigurationSection section, EnvironmentVariablesConfigurationProvider provider)
        {
            var value = section.Value;
            if (value != null && value.StartsWith("${") && value.EndsWith("}"))
            {
                //var variableName = value.Trim('${', '}');
                var variableName = value.Substring(2, value.Length - 3);
                var environmentValue = provider.Get(variableName);

                if (environmentValue != null)
                {
                    section.Value = environmentValue;
                }
            }

            foreach (var child in section.GetChildren())
            { 
                ReplacePlaceHolderWithEnvironmentVariable(child, provider);
            }
        }
    }

    internal class EnvironmentVariablesConfigurationProvider
    {
        public string Get(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
        }
    }
}
