﻿using Ezenity.API.Configurations;
using Ezenity.API.Filters;
using Ezenity.API.Middleware;
using Ezenity.Core.Interfaces;
using Ezenity.Core.Services.Common;
using Ezenity.Infrastructure.Factories;
using Ezenity.Infrastructure.Helpers;
using Ezenity.Infrastructure.Services.Accounts;
using Ezenity.Infrastructure.Services.Emails;
using Ezenity.Infrastructure.Services.EmailTemplates;
using Ezenity.Infrastructure.Services.Sections;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

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
            // Determine the secret key
            string secretKey;
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
            {
                secretKey = Configuration.GetSection("AppSettings")["Secret"];
            }
            else
            {
                // TODO: Insert correct location once on server
                secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? System.IO.File.ReadAllText("./secrete/key.txt").Trim();
            }

            // Configure AppSettings and binds the necessary configuration sections.
            var appSettings = AppSettingsFactory.Create(Configuration, secretKey);
            services.AddSingleton<IAppSettings>(new AppSettingsWrapper(appSettings));

            // Configure ConnectionStringSettings
            var connectionStringSettings = ConnectionStringSettingsFactory.Create(Configuration);
            services.AddSingleton<IConnectionStringSettings>(new ConnectionStringSettingsWrapper(connectionStringSettings));

            // Configure SensitivePropertiesSettings
            var sensitivePropsConfig = SensitivePropertiesSettingsFactory.Create(Configuration);
            services.AddSingleton<ISensitivePropertiesSettings>(new SensitivePropertiesSettingsWrapper(sensitivePropsConfig));

            // Configure the database context
            var connectionString = connectionStringSettings.WebApiDatabase;
            services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

            // TODO: Add support for Azure Key Vault
            // TODO: Add support for AWS Secrets Manager

            // Configure dependecy injection for application services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDataContext, DataContext>();
            services.AddScoped<ITokenHelper, TokenHelper>();

            // Filter DI
            services.AddScoped<LoadAccountFilter>();

            // Singleton services
            services.AddSingleton<FileExtensionContentTypeProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

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
                options.OutputFormatters.Clear();
                options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(new System.Text.Json.JsonSerializerOptions
                {
                    // Configure System.Text.Json settings
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    WriteIndented = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }));

                // Default the supported media type as 'application/vnd.api+json'
                options.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/vnd.api+json");

            });

            services.Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

            // Configure SwaggerGen
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                // Configure API versioning
                services.AddApiVersioning(options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                                        new HeaderApiVersionReader("x-api-version"),
                                                                        new MediaTypeApiVersionReader("x-api-version"));
                });

                // Configure the versioned API explorer
                services.AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VV";
                    options.SubstituteApiVersionInUrl = true;
                });

                services.AddSwaggerGen();

                // Required if you're using Newtonsoft package
                // services.AddSwaggerGenNewtonsoftSupport(); 

                services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "CustomJwt";
                options.DefaultChallengeScheme = "CustomJwt";
            }).AddScheme<AuthenticationSchemeOptions, CustomJwtAuthenticationHandler>("CustomJwt", null);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
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
                app.UseHttpsRedirection();
            }

            // Middleware for serving static files (e.g., CSS, JavaScript, images)
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            // Automatic database migration at startup (Development Only)
            if (env.IsDevelopment())
            {
                using var scope = app.ApplicationServices.CreateScope();
                var dataContext = scope.ServiceProvider.GetService<DataContext>();
                dataContext.Database.Migrate();

                ConfigureSwagger(app, provider, logger);
            }
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
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                // Loop through the API versions and create a swagger endpoint for each
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    logger.LogInformation("ConfigureSwagger() - API Version Info: {descriptionGroupName}", description.GroupName);
                    options.SwaggerEndpoint($"/swagger/api-{description.GroupName}/swagger.json", $"API { description.GroupName.ToUpperInvariant() }");
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

                options.RoutePrefix = String.Empty;
            });

        }
    }
}