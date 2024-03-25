using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Ezenity.API"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .Build();

            Console.WriteLine($"Design Time Factory Class | Environemnt: {environmentName}");

            var builder = new DbContextOptionsBuilder<DataContext>();
            Console.WriteLine($"Design Time Factory Class | Environemnt: {builder}");

            var connectionString = configuration.GetConnectionString("WebApiDatabase");
            Console.WriteLine($"Design Time Factory Class | Environemnt: {connectionString}");

            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new DataContext(builder.Options);
        }
    }
}
