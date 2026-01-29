using Ezenity.Infrastructure.Helpers;
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
            // Use ASPNETCORE_ENVIRONMENT as-is; default to "Production" on servers
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(environmentName))
                environmentName = Environments.Production;

            // Build configuration from various sources.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Log directory (consistent with /srv standard)
            var logDir = Environment.GetEnvironmentVariable("EZENITY_LOG_DIR");
            if (string.IsNullOrWhiteSpace(logDir))
                logDir = "/srv/ezenity/logs/project-ezenity";

            Directory.CreateDirectory(logDir);
            var logFile = Path.Combine(logDir, "api-.log");

            // Configure Serilog for logging.
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Async(a => a.File(
                    logFile,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                ))
                .CreateLogger();

            try
            {
                Console.WriteLine($"Starting web host on Environment: {environmentName}");
                // Build and run the web host.
                CreateHostBuilder(args, configuration).Build().Run();
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
            .ConfigureAppConfiguration(builder =>
            {
                builder.Sources.Clear();
                builder.AddConfiguration(configuration);
            })
            .UseSerilog((_, loggerConfiguration) =>
                loggerConfiguration.ReadFrom.Configuration(configuration))
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                //webBuilder.UseConfiguration(configuration);
            });
    }
}
