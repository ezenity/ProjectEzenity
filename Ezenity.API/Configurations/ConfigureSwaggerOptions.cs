using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Ezenity.API.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ezenity.API.Configurations
{
    /// <summary>
    /// Configures Swagger to accurately represent API versioning and strict JSON:API behavior.
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly ILogger<ConfigureSwaggerOptions> _logger;

        public ConfigureSwaggerOptions(
            IApiVersionDescriptionProvider provider,
            ILogger<ConfigureSwaggerOptions> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public void Configure(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
        {
            // One Swagger document per API version
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = $"Ezenity API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                        Description = "A versatile backend API powering various frontend technologies.",
                        TermsOfService = new Uri("https://example.com/terms"),
                        Contact = new OpenApiContact
                        {
                            Email = "amacallister@ezenity.com",
                            Name = "Anthony MacAllister",
                            Url = new Uri("https://ezenity.com/")
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        }
                    });
            }

            // Match controllers to correct Swagger doc (v1, v2, etc.)
            options.DocInclusionPredicate((documentName, apiDescription) =>
                string.Equals(apiDescription.GroupName, documentName, StringComparison.OrdinalIgnoreCase));

            ConfigureSecurity(options);
            ConfigureOperationFilters(options);
            IncludeXmlComments(options);
        }

        private void ConfigureSecurity(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
        {
            // Single, clean JWT Bearer definition
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme.\n\n" +
                    "Example: `Authorization: Bearer {token}`",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }

        private void ConfigureOperationFilters(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
        {
            // 🔑 CRITICAL FIX:
            // Force x-api-version to be a HEADER, never part of Content-Type
            options.OperationFilter<AddApiVersionHeaderOperationFilter>();

            // Existing filters (kept as-is)
            options.OperationFilter<CreateEmailTemplateFilter>();
            options.OperationFilter<CreateSectionFilter>();
            options.OperationFilter<UpdateAccountFilter>();
            options.OperationFilter<GetEmailTemplateFilter>();
        }

        private void IncludeXmlComments(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        }
    }
}
