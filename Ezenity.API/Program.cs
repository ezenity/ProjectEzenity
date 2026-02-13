using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AutoMapper;
using AutoMapper.Internal;
using Ezenity.API.Filters;
using Ezenity.API.Middleware;
using Ezenity.API.Options;
using Ezenity.Core.Interfaces;
using Ezenity.Core.Options;
using Ezenity.Core.Services.Common;
using Ezenity.Core.Services.Emails;
using Ezenity.Core.Services.Files;
using Ezenity.Infrastructure;
using Ezenity.Infrastructure.Data;
using Ezenity.Infrastructure.Helpers;
using Ezenity.Infrastructure.Security;
using Ezenity.Infrastructure.Factories;
using Ezenity.Infrastructure.Persistence;
using Ezenity.Infrastructure.Services;
using Ezenity.Infrastructure.Services.Accounts;
using Ezenity.Infrastructure.Services.Emails;
using Ezenity.Infrastructure.Services.EmailTemplates;
using Ezenity.Infrastructure.Services.Files;
using Ezenity.Infrastructure.Services.Sections;
using Ezenity.EmailTemplates;
using Google.Apis.Gmail.v1.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Ezenity.Application.Abstractions.Security;
using Ezenity.API.Security;
using Ezenity.Application.Abstractions.Persistence;

namespace Ezenity.API;

public static class Program
{
    public static void Main(string[] args)
    {
        // Use ASPNETCORE_ENVIRONMENT as-is; default to "Production" on servers
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.IsNullOrWhiteSpace(environmentName))
            environmentName = Environments.Production;

        // Build configuration from various sources (your existing behavior)
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

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                EnvironmentName = environmentName,
                ContentRootPath = Directory.GetCurrentDirectory()
            });

            // Replace the default configuration with your custom-built configuration
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddConfiguration(configuration);

            // Serilog
            builder.Host.UseSerilog((_, loggerConfiguration) =>
                loggerConfiguration.ReadFrom.Configuration(builder.Configuration));

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            ConfigurePipeline(app);

            app.Run();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // --- Settings wrappers ---
        var appSettings = AppSettingsFactory.Create(configuration);
        services.AddSingleton<IAppSettings>(new AppSettingsWrapper(appSettings));

        var connectionStringSettings = ConnectionStringSettingsFactory.Create(configuration);
        services.AddSingleton<IConnectionStringSettings>(new ConnectionStringSettingsWrapper(connectionStringSettings));

        var sensitivePropsConfig = SensitivePropertiesSettingsFactory.Create(configuration);
        services.AddSingleton<ISensitivePropertiesSettings>(new SensitivePropertiesSettingsWrapper(sensitivePropsConfig));

        // --- DbContext ---
        var connectionString = connectionStringSettings.WebApiDatabase;
        Console.WriteLine($"Database Connection String: {connectionString}");
        services.AddDbContext<DataContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // --- JSON formatter options (NET 8 TypeInfoResolver requirement) ---
        var jsonFormatterOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };

        services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;

            // Global response types
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status403Forbidden));
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound));
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status409Conflict));
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status422UnprocessableEntity));
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status429TooManyRequests));
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status501NotImplemented));
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status503ServiceUnavailable));

            // Your custom JSON output formatter + media type mapping
            options.OutputFormatters.Clear();
            options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(jsonFormatterOptions));
            options.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/vnd.api+json");
        });

        // Keep MVC JsonOptions consistent
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
        });

        // Razor views from embedded project
        services.AddRazorPages().AddRazorRuntimeCompilation(options =>
        {
            var assembly = typeof(RazorViewRenderer).GetTypeInfo().Assembly;
            var fileProvider = new EmbeddedFileProvider(assembly, "Ezenity.RazorViews");
            options.FileProviders.Add(fileProvider);
        });

        // --- Dependency injection (your current registrations) ---
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IDataContext, DataContext>();
        services.AddScoped<ITokenService, TokenHelper>();

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISectionService, SectionService>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();

        // Application abstractions implemented by Infrastructure
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IAccountRepository, EfAccountRepository>();
        services.AddScoped<IRoleRepository, EfRoleRepository>();
        services.AddScoped<ISectionRepository, EfSectionRepository>();
        services.AddScoped<IEmailTemplateRepository, EfEmailTemplateRepository>();

        services.AddScoped<IPasswordService, PasswordService>();

        // Filters
        services.AddScoped<LoadAccountFilter>();

        // AutoMapper resolvers
        services.AddScoped<RoleResolver>();

        // Singletons
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IEmailTemplateResolver, EmailTemplateResolver>();
        services.AddSingleton<IRazorViewRenderer, RazorViewRenderer>();
        services.AddSingleton<ICurrentUser, CurrentUser>();

        services.AddSingleton<FileExtensionContentTypeProvider>();

        // File storage options
        services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));
        services.PostConfigure<FileStorageOptions>(opts =>
        {
            var envRoot = configuration["EZENITY_FILES_ROOT"];
            if (!string.IsNullOrWhiteSpace(envRoot))
                opts.RootPath = envRoot.Trim();

            var envMax = configuration["EZENITY_FILES_MAX_UPLOAD_BYTES"];
            if (!string.IsNullOrWhiteSpace(envMax) && long.TryParse(envMax, out var maxBytes) && maxBytes > 0)
                opts.MaxUploadBytes = maxBytes;

            var envAllowed = configuration["EZENITY_FILES_ALLOWED_EXTENSIONS"];
            if (!string.IsNullOrWhiteSpace(envAllowed))
            {
                var parts = envAllowed
                    .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();

                if (parts.Count > 0)
                    opts.AllowedExtensions = parts;
            }
        });

        // NOTE: Keeping your exact lifetime (Singleton) to avoid behavior change during migration.
        // After this compiles, we should switch it to AddScoped because it likely uses scoped dependencies.
        services.AddSingleton<IFileStorageService, LocalFileStorageService>();

        // AutoMapper
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.Internal().MethodMappingEnabled = true;
            mc.AddProfile<AutoMapperProfile>();
        });

#if DEVELOPMENT
        mapperConfig.AssertConfigurationIsValid();
#endif

        services.AddSingleton(mapperConfig.CreateMapper());

        // API Versioning
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new MediaTypeApiVersionReader("x-api-version"));
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        // Auth
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "CustomJwt";
            options.DefaultChallengeScheme = "CustomJwt";
        })
        .AddScheme<AuthenticationSchemeOptions, CustomJwtAuthenticationHandler>("CustomJwt", null);

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
        });

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", cors =>
            {
                var originsRaw =
                    configuration["EZENITY_ALLOWED_ORIGINS"]
                    ?? configuration["AllowedOrigins"]
                    ?? "";

                var allowedOrigins = originsRaw
                    .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();

                if (allowedOrigins.Length > 0)
                    cors.WithOrigins(allowedOrigins);

                cors.AllowAnyMethod().AllowAnyHeader();
            });
        });

        services.AddHealthChecks();
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        var env = app.Environment;
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

        app.UseMiddleware<ErrorHandlerMiddleware>();

        if (env.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<JwtMiddleware>();

        ConfigureSwagger(app);

        app.MapControllers();
        app.MapHealthChecks("/health");

        TryApplyMigrations(app, logger);
    }

    private static void ConfigureSwagger(WebApplication app)
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(options =>
        {
            foreach (var description in app.Services
                         .GetRequiredService<IApiVersionDescriptionProvider>()
                         .ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/api/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }

            options.DefaultModelExpandDepth(2);
            options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            options.EnableDeepLinking();
            options.DisplayOperationId();

            options.InjectStylesheet("/assets/custom-ui.css");
            options.RoutePrefix = "swagger";
        });
    }

    private static void TryApplyMigrations(WebApplication app, ILogger logger)
    {
        var apply = Environment.GetEnvironmentVariable("EZENITY_APPLY_MIGRATIONS");
        if (!string.Equals(apply, "true", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("DB migrations skipped (EZENITY_APPLY_MIGRATIONS != true).");
            return;
        }

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();

        logger.LogInformation("Applying DB migrations...");
        db.Database.Migrate();
        logger.LogInformation("DB migrations applied successfully.");
    }
}
