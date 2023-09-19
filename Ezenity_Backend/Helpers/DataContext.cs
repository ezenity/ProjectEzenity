using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Entities.Sections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Ezenity_Backend.Helpers
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the application.
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        /// <summary>
        /// Parameterless constructor for cases requiring manual configuration.
        /// </summary>
        public DataContext() { } // Empty cosntructor for parameterless initialization

        /// <summary>
        /// Invoked to set up the context options such as the database provider and connection string.
        /// </summary>
        /// <param name="optionsBuilder">Used to configure the DbContext.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Check if there is no configuration set for the context
            if (!optionsBuilder.IsConfigured)
            {
                // If no configuration is provided, configure using the default connection string
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                optionsBuilder.UseSqlServer(configuration.GetConnectionString("WebApiDatabase"));
            }
        }

        /// <summary>
        /// Gets or sets the accounts available in the database.
        /// </summary>
        public DbSet<Account> Accounts { get; set; }

        /// <summary>
        /// Gets or sets the roles available in the database.
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Gets or sets the email templates available in the database.
        /// </summary>
        public DbSet<EmailTemplate> EmailTemplates { get; set; }

        /// <summary>
        /// Gets or sets the sections available in the database.
        /// </summary>
        public DbSet<Section> Sections { get; set; }

        /// <summary>
        /// Invoked to configure the database model and relationships.
        /// </summary>
        /// <param name="modelBuilder">Used to map the domain classes to database schema.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurations
            ConfigureAccounts(modelBuilder);
            ConfigureRoles(modelBuilder);
            ConfigureEmailTemplates(modelBuilder);
            ConfigureSections(modelBuilder);

            // Add any relationships between entities if necessary
            // For example, if Section has a one-to-many relationship with another entity called OtherEntity
            // modelBuilder.Entity<Section>()
            //     .HasMany(x => x.OtherEntities)
            //     .WithOne(o => o.Section)
            //     .HasForeignKey(o => o.SectionId);
        }

        /// <summary>
        /// Configures the accounts table schema.
        /// </summary>
        /// <param name="modelBuilder">The model builder for EF Core.</param>
        private void ConfigureAccounts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Accounts");
                entity.HasKey(x => x.Id);

                // Add a relationship with the RefreshToken owned entity
                entity.OwnsMany(account => account.RefreshTokens, refreshToken =>
                {
                    refreshToken.ToTable("RefreshToken"); // Set the table name for RefreshToken
                    refreshToken.Property(rt => rt.Token).IsRequired();
                    refreshToken.Property(rt => rt.Expires).IsRequired();
                    refreshToken.Property(rt => rt.Created).IsRequired();
                    refreshToken.Property(rt => rt.CreatedByIp).IsRequired(false);
                    refreshToken.Property(rt => rt.Revoked).IsRequired(false);
                    refreshToken.Property(rt => rt.RevokedByIp).IsRequired(false);
                    refreshToken.Property(rt => rt.ReplacedByToken).IsRequired(false);
                });
            });
        }

        /// <summary>
        /// Configures the roles table schema.
        /// </summary>
        /// <param name="modelBuilder">The model builder for EF Core.</param>
        private void ConfigureRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(x => x.Id);
            });
        }

        /// <summary>
        /// Configures the email templates table schema.
        /// </summary>
        /// <param name="modelBuilder">The model builder for EF Core.</param>
        private void ConfigureEmailTemplates(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.ToTable("EmailTemplates");
                entity.HasKey(x => x.Id);
            });
        }

        /// <summary>
        /// Configures the sections table schema.
        /// </summary>
        /// <param name="modelBuilder">The model builder for EF Core.</param>
        private void ConfigureSections(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Section>(entity =>
            {
                entity.ToTable("Sections");
                entity.HasKey(x => x.Id);
            });
        }
    }
}
