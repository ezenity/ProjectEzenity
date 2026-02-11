using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AutoMapper;
using AutoMapper.Internal;
using Ezenity.API.Options;
using Ezenity.API.Filters;
using Ezenity.API.Middleware;
using Ezenity.Core.Interfaces;
using Ezenity.Core.Services.Common;
using Ezenity.Core.Services.Emails;
using Ezenity.Core.Services.Files;
using Ezenity.Infrastructure.Data;
using Ezenity.Infrastructure.Factories;
using Ezenity.Infrastructure.Helpers;
using Ezenity.Infrastructure.Services.Accounts;
using Ezenity.Infrastructure.Services.Emails;
using Ezenity.Infrastructure.Services.EmailTemplates;
using Ezenity.Infrastructure.Services.Sections;
using Ezenity.Infrastructure.Services.Files;
using Ezenity.RazorViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Reflection;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Ezenity.API
{
    /// <summary>
    /// Represents the startup configuration of the application where services and the request pipeline are configured.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class with the specified configuration.
        /// </summary>
        /// <param name="configuration">The application configuration properties.</param>
        public Startup(IConfiguration configuration) => Configuration = configuration;

        /// <summary>
        /// Gets the application's configuration properties, including those set in appsettings.json and other sources.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures services for the application. This method gets called by the runtime.
        /// It adds services to the container and configures various options including database context, authentication, API versioning, Swagger, etc.
        /// </summary>
        /// <param name="services">The collection of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure AppSettings and binds the necessary configuration sections.
            var appSettings = AppSettingsFactory.Create(Configuration);
            services.AddSingleton<IAppSettings>(new AppSettingsWrapper(appSettings));

            // Configure ConnectionStringSettings
            var connectionStringSettings = ConnectionStringSettingsFactory.Create(Configuration);
            services.AddSingleton<IConnectionStringSettings>(new ConnectionStringSettingsWrapper(connectionStringSettings));

            // Configure SensitivePropertiesSettings
            var sensitivePropsConfig = SensitivePropertiesSettingsFactory.Create(Configuration);
            services.AddSingleton<ISensitivePropertiesSettings>(new SensitivePropertiesSettingsWrapper(sensitivePropsConfig));

            // Configure the database context
            var connectionString = connectionStringSettings.WebApiDatabase;
            Console.WriteLine($"Database Connection String: {connectionString}");
            services.AddDbContext<DataContext>(options =>
                options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString)));
            // services.AddAWSSecretsManager(Configuration["AWS:SecretsManager:SecretName"]);

            // NOTE: .NET 8 requires TypeInfoResolver when JsonSerializerOptions becomes read-only.
            // Since you're creating JsonSerializerOptions manually for SystemTextJsonOutputFormatter,
            // you MUST set TypeInfoResolver to avoid:
            // "JsonSerializerOptions instance must specify a TypeInfoResolver setting before being marked as read-only."
            var jsonFormatterOptions = new JsonSerializerOptions
            {
                // Configure System.Text.Json settings
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

                // CRITICAL for .NET 8 when the options are "frozen" / made read-only internally:
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            };

            services.AddControllers(options =>
            {
                // Ensure the API returns a 406 Not Acceptable status code if the requested content type is not supported.
                options.ReturnHttpNotAcceptable = true;

                // Global filters for standard HTTP status codes:

                // 400 Bad Request - Indicates that the server cannot process the request due to a client error (e.g., malformed request syntax).
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));

                // 401 Unauthorized - Indicates that the request lacks valid authentication credentials for the target resource.
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));

                // 403 Forbidden - Indicates that the server understood the request but refuses to authorize it.
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status403Forbidden));

                // 404 Not Found - Indicates that the server can't find the requested resource. Links that lead to a 404 page are often called broken or dead links.
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound));

                // 406 Not Acceptable - Indicates that the server cannot produce a response matching the list of acceptable values defined in the request's proactive content negotiation headers.
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));

                // 409 Conflict - Indicates a request conflict with the current state of the target resource (e.g., conflicts in the current state of the resource).
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status409Conflict));

                // 422 Unprocessable Entity - Indicates that the server understands the content type of the request entity, and the syntax of the request entity is correct but it was unable to process the contained instructions.
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status422UnprocessableEntity));

                // 429 Too Many Requests - Indicates the user has sent too many requests in a given amount of time ("rate limiting").
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status429TooManyRequests));

                // 500 Internal Server Error - Indicates a generic error message when an unexpected condition was encountered and no more specific message is suitable.
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));

                // 501 Not Implemented - The server does not support the functionality required to fulfill the request. This can indicate an unrecognized or unsupported request method or feature.
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status501NotImplemented));

                // 503 Service Unavailable - The server is not ready to handle the request, often used for maintenance or overloaded servers. It signifies temporary unavailability, suggesting clients may retry the request after some time.
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status503ServiceUnavailable));

                // Remove all default formatters and add only the JSON:API formatter
                // IMPORTANT (NET 8):
                // We do NOT clear OutputFormatters here anymore, because custom SystemTextJsonOutputFormatter construction
                // can throw due to TypeInfoResolver requirements. We keep the built-in JSON formatter and configure it via AddJsonOptions.
                options.OutputFormatters.Clear();
                //options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(new System.Text.Json.JsonSerializerOptions
                //{
                //    // Configure System.Text.Json settings
                //    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                //    WriteIndented = true,
                //    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                //}));
                options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(jsonFormatterOptions));

                // Default the supported media type as 'application/vnd.api+json'
                options.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/vnd.api+json");

            });
            //.AddJsonOptions(options =>
            //{
            //    // Configure System.Text.Json settings
            //    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            //    options.JsonSerializerOptions.WriteIndented = true;
            //    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            //});

            // Add minimal MVC services required for Razor views
            services.AddRazorPages().AddRazorRuntimeCompilation(options =>
            {
              var assembly = typeof(RazorViewRenderer).GetTypeInfo().Assembly;
              var fileProvider = new EmbeddedFileProvider(assembly, "Ezenity.RazorViews");
              options.FileProviders.Add(fileProvider);
            });

            // TODO: Add support for Azure Key Vault
            // services.AddAzureKeyValut(Configuration["KeyValut:Uri"]);

            // TODO: Add support for AWS Secrets Manager
            // services.AddAWSSecretsManager(Configuration["AWS:SecretsManager:SecretName"]);

            // Configure dependecy injection for application services
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IDataContext, DataContext>();
            services.AddScoped<ITokenHelper, TokenHelper>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISectionService, SectionService>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();

            // Filter DI
            services.AddScoped<LoadAccountFilter>();

            // Automapper Resolvers
            services.AddScoped<RoleResolver>();

            // Singleton services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IEmailTemplateResolver, EmailTemplateResolver>();
            //services.AddSingleton<IConfigurationUpdater, ConfigurationUpdater>();
            services.AddSingleton<IRazorViewRenderer, RazorViewRenderer>();

            // Content type provider is stateless and safe as singleton
            services.AddSingleton<FileExtensionContentTypeProvider>();

            // File storage options + JSON options
            services.Configure<FileStorageOptions>(Configuration.GetSection("FileStorage"));

            // Optional: Allow env override in your EZENITY_* naming style
            services.PostConfigure<FileStorageOptions>(opts =>
            {
                var envRoot = Configuration["EZENITY_FILES_ROOT"];
                if (!string.IsNullOrWhiteSpace(envRoot))
                    opts.RootPath = envRoot.Trim();

                var envMax = Configuration["EZENITY_FILES_MAX_UPLOAD_BYTES"];
                if (!string.IsNullOrWhiteSpace(envMax) && long.TryParse(envMax, out var maxBytes) && maxBytes > 0)
                    opts.MaxUploadBytes = maxBytes;

                var envAllowed = Configuration["EZENITY_FILES_ALLOWED_EXTENSIONS"];
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

            services.Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                // Keep it consistent with the formatter requirement in .NET 8
                options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
            });

            // IMPORTANT: File storage service must be SCOPED (it uses DataContext)
            services.AddSingleton<IFileStorageService, LocalFileStorageService>();

            // AutoMapper
            //services.AddAutoMapper(typeof(AutoMapperProfile).Assembly); // .NET 5.0 -
            var mapperConfig = new MapperConfiguration(mc =>
            {
                // Needed for https://github.com/AutoMapper/AutoMapper/issues/3988
                mc.Internal().MethodMappingEnabled = true;
                // mc.AddProfile(new AutoMapperProfile());
                mc.AddProfile<AutoMapperProfile>();
                // mc.AddMaps(typeof(AutoMapperProfile).Assembly);
            });

#if DEVELOPMENT
            mapperConfig.AssertConfigurationIsValid();
#endif
            services.AddSingleton(mapperConfig.CreateMapper());

            // Configure SwaggerGen
            //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            //{
                // Configure API versioning
                services.AddApiVersioning(options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new UrlSegmentApiVersionReader(),
                        new HeaderApiVersionReader("x-api-version"),
                        new MediaTypeApiVersionReader("x-api-version"));
                }).AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VV";
                    options.SubstituteApiVersionInUrl = true;
                });

                // Helps Swashbuckle discover endpoints in many setups
                services.AddEndpointsApiExplorer();

                services.AddSwaggerGen();

                // Required if we're using Newtonsoft package
                // services.AddSwaggerGenNewtonsoftSupport();

                services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            //};

            // authn/authZ
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "CustomJwt";
                options.DefaultChallengeScheme = "CustomJwt";
            }).AddScheme<AuthenticationSchemeOptions, CustomJwtAuthenticationHandler>("CustomJwt", null);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });

            // CORS, reading from config for consistency
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    // This reads your appsettings.json/appsettings.Production.json "AllowedOrigins"
                    // which may itself be substituted from ${EZENITY_ALLOWED_ORIGINS}
                    var originsRaw =
                        Configuration["EZENITY_ALLOWED_ORIGINS"]
                        ?? Configuration["AllowedOrigins"]
                        ?? "";

                    var allowedOrigins = originsRaw
                        .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToArray();

                    if (allowedOrigins.Length > 0)
                    {
                        builder.WithOrigins(allowedOrigins);
                    }

                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // Configure Health Checks
            services.AddHealthChecks();
        }

        /// <summary>
        /// Configures the HTTP request pipeline. This method gets called by the runtime.
        /// It adds middleware components to the request pipeline such as error handling, static files, routing, authentication, authorization, Swagger, etc.
        /// </summary>
        /// <param name="app">Defines a class that provides mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        /// <param name="provider">Provides a mechanism to iterate API versions and their respective descriptions.</param>
        /// <param name="logger">Represents a type used to perform logging.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, ILogger<Startup> logger)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();

            if (env.IsProduction())
            {
                // Only enable if behind TLS (Nginx terminated SSL)
                app.UseHttpsRedirection();
                //app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains());
            }

            // Middleware for serving static files (e.g., CSS, JavaScript, images)
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<JwtMiddleware>();

            // Swagger (can later limit to non-prod)
            ConfigureSwagger(app, provider, logger);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            // Optional migrations on startup (controlled)
            TryApplyMigrations(app, env, logger);
        }

        /// <summary>
        /// Configures SwaggerGen and SwaggerUI services. This method sets up Swagger documentation for different API versions and configures Swagger UI options.
        /// </summary>
        /// <param name="app">Defines a class that provides mechanisms to configure an application's request pipeline.</param>
        /// <param name="provider">Provides a mechanism to iterate API versions and their respective descriptions.</param>
        /// <param name="logger">Represents a type used to perform logging.</param>
        private static void ConfigureSwagger(IApplicationBuilder app, IApiVersionDescriptionProvider provider, ILogger<Startup> logger)
        {
            // Add Swagger and Swagger UI here using the provided IApiVersionDescrptionProvider
            //app.UseSwagger();
            app.UseSwagger(c =>
            {
                // This makes swagger.json live under /api/swagger/{documentName}/swagger.json
                c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                // Loop through the API versions and create a swagger endpoint for each
                //foreach (var description in provider.ApiVersionDescriptions)
                foreach (var description in app
                                        .ApplicationServices
                                        .GetRequiredService<IApiVersionDescriptionProvider>()
                                        .ApiVersionDescriptions
                        )
                {
                    //logger.LogInformation("Swagger API Version: {GroupName}", description.GroupName);
                    options.SwaggerEndpoint($"/api/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }

                // options.OAuthClientId("");
                // options.OAuthAppName("");
                // options.OAuthUsePkce();

                options.DefaultModelExpandDepth(2);
                options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Default = List
                options.EnableDeepLinking();
                options.DisplayOperationId();

                options.InjectStylesheet("/assets/custom-ui.css");

                options.RoutePrefix = "swagger";
            });
        }

        private static void TryApplyMigrations(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // Only run if explicitly enabled
            var apply = Environment.GetEnvironmentVariable("EZENITY_APPLY_MIGRATIONS");
            if (!string.Equals(apply, "true", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("DB migrations skipped (EZENITY_APPLY_MIGRATIONS != true).");
                return;
            }

            try
            {
                using var scope = app.ApplicationServices.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                logger.LogInformation("Applying DB migrations...");
                db.Database.Migrate();
                logger.LogInformation("DB migrations applied successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DB migration failed.");
                // In prod you probably want to fail hard so you know immediately
                throw;
            }
        }
    }
}
