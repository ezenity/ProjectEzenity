using Ezenity_Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ezenity_Backend.Helpers
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connection to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Skill> Skills { get; set; }
    }
}
