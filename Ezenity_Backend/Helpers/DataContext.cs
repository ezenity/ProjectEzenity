using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Entities.Sections;
using Microsoft.EntityFrameworkCore;

namespace Ezenity_Backend.Helpers
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the application.
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the "DataContext" class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

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
        /// Overrides the OnModelCreating method to configure the entity relationships and table schema in the database.
        /// </summary>
        /// <param name="modelBuilder">The model builder used by Entity Framework Core to configure the entity relationships and schema.</param>
        /// <remarks>
        /// This method does the following:
        /// - Calls the ConfigureAccounts method to set up the Accounts table.
        /// - Calls the ConfigureRefreshtoken method to set up the RefreshTokens table.
        /// - Calls the ConfigureRoles method to set up the Roles table.
        /// - Calls the ConfigureEmailTemplates method to set up the EmailTemplates table.
        /// - Calls the ConfigureSections method to set up the Sections table.
        /// - Additional relationships between entities can be added as necessary.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurations
            ConfigureAccounts(modelBuilder);
            ConfigureRefreshtoken(modelBuilder); // new
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
        /// Configures the schema for the Accounts table in the database.
        /// This method sets up various properties and relationships for the Account entity.
        /// </summary>
        /// <param name="modelBuilder">The model builder used by Entity Framework Core to configure the entity.</param>
        /// <remarks>
        /// - Sets the primary key for the Accounts table as 'Id'.
        /// - Establishes a many-to-one relationship between the Account and Role entities.
        ///   Each Account has one Role, while a Role can be associated with multiple Accounts.
        ///   The relationship is enforced by a foreign key 'RoleId' in the Account entity.
        /// </remarks>
        private void ConfigureAccounts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Accounts");
                entity.HasKey(x => x.Id);

                entity.HasOne(a => a.Role)
                      .WithMany()
                      .HasForeignKey(a => a.RoleId);
            });

        }

        /// <summary>
        /// Configures the schema for the RefreshTokens table in the database.
        /// This method sets up various properties, their requirements, and establishes the relationship
        /// between the RefreshToken entity and the Account entity.
        /// </summary>
        /// <param name="modelBuilder">The model builder used by Entity Framework Core to configure the entity.</param>
        /// <remarks>
        /// - Sets the primary key for the RefreshTokens table as 'Id'.
        /// - Sets the 'Token', 'Expires', and 'Created' properties to be required.
        /// - Makes 'CreatedByIp', 'Revoked', 'RevokedByIp', and 'ReplacedByToken' as optional fields.
        /// - Establishes a one-to-many relationship between the Account entity and RefreshToken entity. 
        ///   Each Account can have multiple RefreshTokens, and each RefreshToken is linked back to an Account
        ///   via a foreign key 'AccountId'.
        /// </remarks>
        private void ConfigureRefreshtoken(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Token).IsRequired();
                entity.Property(x => x.Expires).IsRequired();
                entity.Property(x => x.Created).IsRequired();
                entity.Property(x => x.CreatedByIp).IsRequired(false);
                entity.Property(x => x.Revoked).IsRequired(false);
                entity.Property(x => x.RevokedByIp).IsRequired(false);
                entity.Property(x => x.ReplacedByToken).IsRequired(false);

                entity.HasOne(x => x.Account)
                      .WithMany(x => x.RefreshTokens)
                      .HasForeignKey(x => x.AccountId);
            });
        }

        /// <summary>
        /// Configures the schema for the Roles table in the database.
        /// This method sets up various properties and establishes relationships
        /// between the Role entity and the Account entity.
        /// </summary>
        /// <param name="modelBuilder">The model builder used by Entity Framework Core to configure the entity.</param>
        /// <remarks>
        /// - Sets the primary key for the Roles table as 'Id'.
        /// - Establishes a one-to-many relationship between the Role entity and the Account entity.
        ///   Each Role can be associated with multiple Accounts, and each Account has a Role.
        ///   The relationship is enforced by a foreign key 'RoleId' in the Account entity.
        /// </remarks>
        private void ConfigureRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(x => x.Id);

                entity.HasMany(r => r.Accounts)
                      .WithOne(a => a.Role)
                      .HasForeignKey(a => a.RoleId);
            });
        }

        /// <summary>
        /// Configures the schema for the EmailTemplates table in the database.
        /// This method sets the primary key for the EmailTemplate entity.
        /// </summary>
        /// <param name="modelBuilder">The model builder used by Entity Framework Core to configure the entity.</param>
        /// <remarks>
        /// - Sets the primary key for the EmailTemplates table as 'Id'.
        /// </remarks>
        private void ConfigureEmailTemplates(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.ToTable("EmailTemplates");
                entity.HasKey(x => x.Id);
            });
        }

        /// <summary>
        /// Configures the schema for the Sections table in the database.
        /// This method sets the primary key for the Section entity.
        /// </summary>
        /// <param name="modelBuilder">The model builder used by Entity Framework Core to configure the entity.</param>
        /// <remarks>
        /// - Sets the primary key for the Sections table as 'Id'.
        /// </remarks>
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
