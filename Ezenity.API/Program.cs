using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace Ezenity.API
{
    /// <summary>
    /// Main entry point for the Ezenity.API web application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry method for the web application. Configures and launches the web host.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            // Determine the environment based on the 'ASPNETCORE_ENVIRONMENT' variable.
            /*var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production"
                ? Environments.Development
                : Environments.Production;*/
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            // Build configuration from various sources.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Configure Serilog for logging.
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Console.WriteLine("Starting web host");
                // Build and run the web host.
                //CreateHostBuilder(args, configuration).Build().Run();
                var hostBuilder = CreateHostBuilder(args, configuration)
                        .UseUpdatedConfiguration();
            }
            catch (Exception ex)
            {
                // Log fatal errors and terminate the application.
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                // Flush and close the log before application shutdown.
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Configures and creates a host builder for the web application.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        /// <param name="configuration">Application configuration settings.</param>
        /// <returns>A configured IHostBuilder instance.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                //webBuilder.UseConfiguration(configuration);
            });
    }
}
