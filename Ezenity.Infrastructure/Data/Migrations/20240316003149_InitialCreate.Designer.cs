﻿// <auto-generated />
using System;
using Ezenity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Ezenity.Infrastructure.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240316003149_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Ezenity.Core.Entities.Accounts.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("AcceptTerms")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("PasswordReset")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ResetToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("ResetTokenExpires")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Verified")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Accounts", (string)null);
                });

            modelBuilder.Entity("Ezenity.Core.Entities.Accounts.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreatedByIp")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ReplacedByToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("RevokedByIp")
                        .HasColumnType("longtext");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("RefreshTokens", (string)null);
                });

            modelBuilder.Entity("Ezenity.Core.Entities.Accounts.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("Ezenity.Core.Entities.EmailTemplates.EmailTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsDynamic")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("PlaceholderValuesJson")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TemplateName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("EmailTemplates", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Content = "Hello firstNameValue, This email is already registered.",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDefault = true,
                            IsDynamic = true,
                            PlaceholderValuesJson = "{\"firstNameValue\":\"\"}",
                            StartDate = new DateTime(2023, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Subject = "Email Already Registered",
                            TemplateName = "alreadyRegistered"
                        },
                        new
                        {
                            Id = 2,
                            Content = "Hello firstNameValue, please verify your email here: verificationUrl",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EndDate = new DateTime(2024, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDefault = true,
                            IsDynamic = true,
                            PlaceholderValuesJson = "{\"firstNameValue\":\"\",\"verificationUrl\":\"http://localhost/account/verify-email?token={token}\"}",
                            StartDate = new DateTime(2023, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Subject = "Email Verification",
                            TemplateName = "verification"
                        },
                        new
                        {
                            Id = 3,
                            Content = "Hello firstNameValue, <br>please verify your email here: verificationUrl",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EndDate = new DateTime(2024, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDefault = true,
                            IsDynamic = true,
                            PlaceholderValuesJson = "{\"firstNameValue\":\"\",\"resetUrl\":\"http://localhost/account/reset-password?token={token}\"}",
                            StartDate = new DateTime(2023, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Subject = "Forgot Password - Reset Password",
                            TemplateName = "passwordReset"
                        });
                });

            modelBuilder.Entity("Ezenity.Core.Entities.Sections.Section", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AccessLevel")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Layout")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MetaTags")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("ParentSectionId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Sections", (string)null);
                });

            modelBuilder.Entity("Ezenity.Core.Entities.Accounts.Account", b =>
                {
                    b.HasOne("Ezenity.Core.Entities.Accounts.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Ezenity.Core.Entities.Accounts.RefreshToken", b =>
                {
                    b.HasOne("Ezenity.Core.Entities.Accounts.Account", "Account")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Ezenity.Core.Entities.Accounts.Account", b =>
                {
                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("Ezenity.Core.Entities.Accounts.Role", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}