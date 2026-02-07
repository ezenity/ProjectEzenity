using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Ezenity.API.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ezenity.API.Configurations
{
    /// <summary>
    /// Configures Swagger to accurately represent API versioning in the generated Swagger documents, enhancing the API documentation with version details and additional configuration settings.
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly ILogger<ConfigureSwaggerOptions> _logger;

        /// <summary>
        /// Constructs an instance of <see cref="ConfigureSwaggerOptions"/> with dependencies injected through parameters.
        /// </summary>
        /// <param name="provider">Provides API version description, used to enumerate and document different API versions.</param>
        /// <param name="logger">Facilitates logging for diagnostic and informational purposes throughout the configuration process.</param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, ILogger<ConfigureSwaggerOptions> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        /// <summary>
        /// Configures the options for Swagger generation, including the creation of separate Swagger documents for each API version and the setup of security definitions.
        /// </summary>
        /// <param name="options">The options to be configured for Swagger generation.</param>
        public void Configure(SwaggerGenOptions options)
        {
            // Documentation for each version
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                _logger.LogInformation("CustomSwaggerOptions() | ForEach - Description Api Version: {descriptionGroupName}", description.GroupName);

                options.SwaggerDoc(
                    //$"api-{description.GroupName}",
                    description.GroupName, // doc name is "v1", "v2", etc.
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
                            Url = new Uri("https://Ezenity.com/")
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        }
                    });
            }

            options.DocInclusionPredicate((documentName, apiDescription) =>
            {
                //return apiDescription
                //            .TryGetMethodInfo(out MethodInfo methodInfo) &&
                //       methodInfo
                //            .DeclaringType
                //            .GetCustomAttributes(true)
                //            .OfType<ApiVersionAttribute>()
                //            .SelectMany(attribute => attribute.Versions)
                //            .Any(v => $"api-v{v}" == documentName);

                // With ApiExplorer GroupNameFormat = "'v'VVV", apiDescription.GroupName is "v1", "v2", etc.
                return string.Equals(apiDescription.GroupName, documentName, StringComparison.OrdinalIgnoreCase);

            });

            ConfigureSwaggerSecurity(options);
            ConfigureSwaggerFilters(options);
            IncludeXmlComments(options);
        }

        /// <summary>
        /// Configures Swagger security definitions, setting up JWT authentication for the API documentation.
        /// </summary>
        /// <param name="options">The Swagger generation options to be further configured.</param>
        private void ConfigureSwaggerSecurity(SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'. Use this format when making authenticated API calls. You can obtain the JWT Bearer token by making a POST request to http://localhost:5000/api/v1/accounts/authenticate.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    Array.Empty<string>()
                }
            }); ;
        }

        /// <summary>
        /// Adds custom operation filters to Swagger options for handling specific documentation needs, such as annotating API operations with additional metadata or parameters.
        /// </summary>
        /// <param name="options">The Swagger generation options to be further configured.</param>
        private void ConfigureSwaggerFilters(SwaggerGenOptions options)
        {
            // Create Operational Filters
            options.OperationFilter<CreateEmailTemplateFilter>();
            options.OperationFilter<CreateSectionFilter>();

            // Update Operational Filters
            options.OperationFilter<UpdateAccountFilter>();

            // Get Operational Filters
            options.OperationFilter<GetEmailTemplateFilter>();
        }

        /// <summary>
        /// Includes XML comment files for Swagger documentation, enhancing API descriptions with comments from the source code.
        /// </summary>
        /// <param name="options">The Swagger generation options to be further configured.</param>
        private void IncludeXmlComments(SwaggerGenOptions options)
        {
            // Include XML comments if they exist
            var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
            if (File.Exists(xmlCommentsFullPath))
            {
                options.IncludeXmlComments(xmlCommentsFullPath);
            }
        }

    }
}
