using Ezenity_Backend.Entities;
using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Entities.Common;
using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Entities.Sections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Ezenity_Backend.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DataContext() { } // Empty cosntructor for parameterless initialization


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

        public DbSet<Account> Accounts { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Section> Sections { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurations
            ConfigureAccounts(modelBuilder);
            ConfigureEmailTemplates(modelBuilder);
            ConfigureSections(modelBuilder);

            // Add any relationships between entities if necessary
            // For example, if Section has a one-to-many relationship with another entity called OtherEntity
            // modelBuilder.Entity<Section>()
            //     .HasMany(x => x.OtherEntities)
            //     .WithOne(o => o.Section)
            //     .HasForeignKey(o => o.SectionId);
        }

        private void ConfigureAccounts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IAccount>(entity =>
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

        private void ConfigureEmailTemplates(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IEmailTemplate>(entity =>
            {
                entity.ToTable("EmailTemplates");
                entity.HasKey(x => x.Id);
            });
        }

        private void ConfigureSections(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ISection>(entity =>
            {
                entity.ToTable("Sections");
                entity.HasKey(x => x.Id);
            });
        }
    }
}
