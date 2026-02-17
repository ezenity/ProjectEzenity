using Ezenity.Infrastructure.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ezenity.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Ezenity.API");
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Create the settings objects
            var connectionStringSettings = ConnectionStringSettingsFactory.Create(configuration);

            // Directly use the returned settings
            var connectionString = connectionStringSettings.WebApiDatabase;

            Console.WriteLine($"DesignTime Factory | Database Connection String: {connectionString}");

            var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new DataContext(builder.Options);
        }
    }
}
