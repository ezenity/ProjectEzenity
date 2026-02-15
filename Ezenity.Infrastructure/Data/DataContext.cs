using Ezenity.Domain.Entities.Accounts;
using Ezenity.Domain.Entities.EmailTemplates;
using Ezenity.Domain.Entities.Files;
using Ezenity.Domain.Entities.Sections;
using Ezenity.Domain.Entities.Vault;
using Ezenity.Domain.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System;

namespace Ezenity.Infrastructure.Data;

/// <summary>
/// EF Core database context.
/// Infrastructure owns EF mappings and persistence.
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

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    /// <summary>
    /// Gets or sets the email templates available in the database.
    /// </summary>
    public DbSet<EmailTemplate> EmailTemplates { get; set; }

    /// <summary>
    /// Gets or sets the sections available in the database.
    /// </summary>
    public DbSet<Section> Sections { get; set; }

    /// <summary>
    /// Files
    /// </summary>
    public DbSet<FileAsset> FileAssets { get; set; }

    /// <summary>
    /// Vault
    /// </summary>
    public DbSet<AccountEmblem> AccountEmblems { get; set; }
    public DbSet<VaultEmblem> VaultEmblems { get; set; }
    public DbSet<VaultMission> VaultMissions { get; set; }
    public DbSet<VaultMissionReward> VaultMissionRewards { get; set; } = null!;
    public DbSet<VaultMissionEmblemReward> VaultMissionEmblemRewards { get; set; }
    public DbSet<VaultMissionSubmission> VaultMissionSubmissions { get; set; }
    public DbSet<VaultMissionCompletion> VaultMissionCompletions { get; set; }
    public DbSet<VaultMissionComment> VaultMissionComments { get; set; }

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
        base.OnModelCreating(modelBuilder);

        // Configurations
        ConfigureAccounts(modelBuilder);
        ConfigureRoles(modelBuilder);
        ConfigureRefreshtoken(modelBuilder); // new

        ConfigureEmailTemplates(modelBuilder);
        ConfigureSections(modelBuilder);

        // Files metadata table (FileAssets)
        ConfigureFiles(modelBuilder);

        // Vault tables
        ConfigureVault(modelBuilder);

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
                  .WithMany(r => r.Accounts)
                  .HasForeignKey(a => a.RoleId);
        });

        modelBuilder.Entity<AccountEmblem>(entity =>
        {
            entity.ToTable("AccountEmblems");
            entity.HasKey(x => new { x.AccountId, x.VaultEmblemId });

            entity.Property(x => x.ObtainedUtc).IsRequired();
            entity.Property(x => x.Note).HasMaxLength(1000);

            entity.HasOne(x => x.Account)
                  .WithMany(x => x.Emblems)
                  .HasForeignKey(x => x.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Emblem)
                  .WithMany(x => x.EarnedBy)
                  .HasForeignKey(x => x.VaultEmblemId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ObtainedFromVaultMission)
                  .WithMany()
                  .HasForeignKey(x => x.ObtainedFromVaultMissionId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(x => x.ObtainedUtc);
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
                  .HasForeignKey(x => x.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);
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

            entity.HasIndex(x => x.Name).IsUnique();

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

            // If TemplateName must be unique, make it unique:
            entity.HasIndex(x => x.TemplateName).IsUnique();

            // Seed data (keep if we want it)
            entity.HasData(
                new EmailTemplate
                {
                    Id = 1,
                    TemplateName = "AlreadyRegistered",
                    Subject = "Email Already Registered",
                    ContentViewPath = "EmailTemplates/Templates",
                    IsDefault = true,
                    IsDynamic = true,
                    StartDate = new DateTime(2023, 8, 18),
                    PlaceholderValuesJson = "{\"firstName\": \"\"}"
                },
                new EmailTemplate
                {
                    Id = 2,
                    TemplateName = "EmailVerification",
                    Subject = "Email Verification",
                    ContentViewPath = "EmailTemplates/Templates",
                    IsDefault = true,
                    IsDynamic = true,
                    StartDate = new DateTime(2023, 8, 18),
                    EndDate = new DateTime(2024, 8, 18),
                    PlaceholderValuesJson = "{\"firstName\": \"\", \"templateTitle\": \"\", \"bodyContent\": \"Hello {firstName}, please verify your email here: {verificationUrl}\", \"verificationUrl\": \"{origin}/account/verify-email?token={account.VerificationToken}\"}"
                },
                new EmailTemplate
                {
                    Id = 3,
                    TemplateName = "PasswordReset",
                    Subject = "Forgot Password - Reset Password",
                    ContentViewPath = "EmailTemplates/Templates",
                    IsDefault = true,
                    IsDynamic = true,
                    StartDate = new DateTime(2023, 8, 18),
                    EndDate = new DateTime(2024, 8, 18),
                    PlaceholderValuesJson = "{\"firstName\": \"\", \"resetUrl\": \"http://localhost/account/reset-password?token={token}\"}"
                });
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

            // If Title is unique in your service validation, enforce it here too:
            entity.HasIndex(x => x.Title).IsUnique();
        });
    }

    /// <summary>
    /// Configures the database schema for all file/media storage entities.
    /// </summary>
    /// <remarks>
    /// This configuration is responsible for mapping your file-related domain to tables that support:
    /// - Persisting **file metadata** (stable IDs, original name, stored name/path, content type, size, scope, created timestamps, etc.)
    /// - Fast retrieval for UI use-cases (listing, filtering by scope/owner/type/date) via **indexes**
    /// - Relationships to the rest of the app (e.g., uploader account, profile media, vault submissions/attachments)
    /// - Safe delete behavior (commonly: avoid accidental cascading deletes of “blobs” when domain rows are removed)
    ///
    /// The actual bytes typically live in filesystem/object storage; the DB stores metadata + relations.
    /// </remarks>
    /// <param name="modelBuilder">The model builder used by Entity Framework Core.</param>
    private void ConfigureFiles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileAsset>(entity =>
        {
            entity.ToTable("FileAssets");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.OriginalName)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(x => x.StoredName)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(x => x.ContentType)
                  .IsRequired()
                  .HasMaxLength(127);

            entity.Property(x => x.Size)
                  .IsRequired();

            entity.Property(x => x.Scope)
                  .IsRequired(false)
                  .HasMaxLength(64);

            entity.Property(x => x.CreatedUtc)
                  .IsRequired();

            entity.HasIndex(x => x.StoredName).IsUnique();
            entity.HasIndex(x => new { x.Scope, x.CreatedUtc });

            // FileAsset has CreatedByAccountId + navigation CreatedByAccount
             entity.HasOne(x => x.CreatedByAccount)
                   .WithMany()
                   .HasForeignKey(x => x.CreatedByAccountId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
        });
    }

    /// <summary>
    /// Configures the database schema for all Vault domain entities (missions, milestones, objectives, rewards, and user progress).
    /// </summary>
    /// <remarks>
    /// This configuration supports “THE VAULT” feature set, typically including:
    /// - Defining vault content (missions/milestones/tasks/objectives) and their presentation/order/visibility
    /// - Reward definitions (rep, coins, emblem/coin unlocks) tied to vault items
    /// - User state (completion/approval status, timestamps, audit trail)
    /// - User-generated content for verification (submissions/attachments like uploaded media, external links)
    /// - Social proof & moderation (comments/posts, approvals, rejections, flags, etc.)
    ///
    /// The goal is a clean relational model that lets the frontend query:
    /// - What exists (vault catalog)
    /// - What a user has done (progress + earned unlocks)
    /// - What needs review (pending submissions)
    /// </remarks>
    /// <param name="modelBuilder">The model builder used by Entity Framework Core.</param>
    private void ConfigureVault(ModelBuilder modelBuilder)
    {
        /* Vault Emblems & Vault Emblem Rarity */
        modelBuilder.Entity<VaultEmblem>(entity =>
        {
            entity.ToTable("VaultEmblems");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Slug)
                  .IsRequired()
                  .HasMaxLength(64);

            entity.HasIndex(x => x.Slug)
                  .IsUnique();

            entity.Property(x => x.Name)
                  .IsRequired()
                  .HasMaxLength(120);

            entity.Property(x => x.Description)
                  .IsRequired(false)
                  .HasMaxLength(1000);

            entity.Property(x => x.SeasonTag)
                  .HasMaxLength(80);

            // Enum handling:
            // Long-term stable: store enum as int
            entity.Property(x => x.Rarity)
                  .HasConversion<int>()
                  .IsRequired();

            entity.Property(x => x.IsActive)
                  .IsRequired();

            entity.Property(x => x.SortOrder)
                  .IsRequired();

            entity.Property(x => x.CreatedUtc)
                  .IsRequired();

            entity.HasOne(x => x.IconFileAsset)
                  .WithMany()
                  .HasForeignKey(x => x.IconFileAssetId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Optional but recommended indexes for common queries:
            entity.HasIndex(x => x.IsActive);
            entity.HasIndex(x => new { x.SeasonTag, x.SortOrder });
        });

        /* Vault Mission */
        modelBuilder.Entity<VaultMission>(entity =>
        {
            entity.ToTable("VaultMissions");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Slug).IsRequired().HasMaxLength(64);
            entity.HasIndex(x => x.Slug).IsUnique();

            entity.Property(x => x.Title).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(4000).IsRequired(false);
            entity.Property(x => x.IsActive).IsRequired();
            entity.Property(x => x.CreatedUtc).IsRequired();

            entity.HasIndex(x => x.IsActive);
            entity.HasIndex(x => x.CreatedUtc);
        });

        /* Vault Mission Comment */
        modelBuilder.Entity<VaultMissionComment>(entity =>
        {
            entity.ToTable("VaultMissionComments");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Body).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.CreatedUtc).IsRequired();

            entity.HasOne(x => x.Mission)
                  .WithMany()
                  .HasForeignKey(x => x.MissionId);

            entity.HasOne(x => x.Account)
                  .WithMany()
                  .HasForeignKey(x => x.AccountId);

            entity.HasIndex(x => new { x.MissionId, x.CreatedUtc });
        });

        /* Vault Mission Completion */
        modelBuilder.Entity<VaultMissionCompletion>(entity =>
        {
            entity.ToTable("VaultMissionCompletions");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status).HasMaxLength(20).IsRequired(); // Submitted/Approved/Rejected
            entity.Property(x => x.CreatedUtc).IsRequired();

            entity.HasOne(x => x.Mission)
                  .WithMany()
                  .HasForeignKey(x => x.MissionId);

            entity.HasOne(x => x.Account)
                  .WithMany()
                  .HasForeignKey(x => x.AccountId);

            entity.HasIndex(x => new { x.MissionId, x.AccountId }).IsUnique(); // one completion record per user per mission
        });

        /* Vault Mission Emblem Reward */
        modelBuilder.Entity<VaultMissionEmblemReward>(entity =>
        {
            entity.ToTable("VaultMissionEmblemRewards");
            entity.HasKey(x => new { x.VaultMissionId, x.VaultEmblemId });

            entity.Property(x => x.Quantity).IsRequired();
            entity.Property(x => x.IsPrimary).IsRequired();

            entity.HasOne(x => x.Mission)
                  .WithMany(x => x.EmblemRewards)
                  .HasForeignKey(x => x.VaultMissionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Emblem)
                  .WithMany(x => x.MissionRewards)
                  .HasForeignKey(x => x.VaultEmblemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        /* Vault Mission Reward */
        modelBuilder.Entity<VaultMissionReward>(entity =>
        {
            entity.ToTable("VaultMissionRewards");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Coins).IsRequired();
            entity.Property(x => x.Rep).IsRequired();

            entity.HasOne(x => x.Mission)
                  .WithMany(m => m.Rewards)
                  .HasForeignKey(x => x.VaultMissionId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Typical helpful index for joins
            entity.HasIndex(x => x.VaultMissionId);
        });

        /* Vault Mission Submission */
        modelBuilder.Entity<VaultMissionSubmission>(entity =>
        {
            entity.ToTable("VaultMissionSubmissions");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Platform).HasMaxLength(30).IsRequired() // YouTube/Instagram/TikTok/Facebook/Upload
            entity.Property(x => x.Url).HasMaxLength(2000);
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.Property(x => x.CreatedUtc).IsRequired();

            entity.HasOne(x => x.Mission)
                  .WithMany()
                  .HasForeignKey(x => x.MissionId);

            entity.HasOne(x => x.Account)
                  .WithMany()
                  .HasForeignKey(x => x.AccountId);

            entity.HasOne(x => x.File)
                  .WithMany()
                  .HasForeignKey(x => x.FileAssetId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(x => new { x.MissionId, x.AccountId, x.CreatedUtc });
        });

        /* Vault Mission Submission Media */

        /* Vault Mission Submission Status */

    }

}
