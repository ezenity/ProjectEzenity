using Ezenity.Core.Interfaces;
using Ezenity.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ezenity.API
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseUpdatedConfiguration(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((context, services) =>
            {
                // Register IConfiguration as a singleton
                services.AddSingleton<IConfigurationUpdater, ConfigurationUpdater>();

                // Execute configuration update after all services are registered
                var serviceProvider = services.BuildServiceProvider();
                var configurationUpdater = serviceProvider.GetRequiredService<IConfigurationUpdater>();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                configurationUpdater.UpdateConfiguration(configuration);
            });
        }
    }
}
