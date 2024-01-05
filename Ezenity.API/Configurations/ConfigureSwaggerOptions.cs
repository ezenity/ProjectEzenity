using Ezenity.API.Filters;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
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
    /// Configures Swagger to understand and represent API versioning in the generated Swagger documents.
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly ILogger<ConfigureSwaggerOptions> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        /// <param name="provider">The API version description provider used for retrieving API version information.</param>
        /// <param name="logger">The logger used for logging information within this configuration.</param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, ILogger<ConfigureSwaggerOptions> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        /// <summary>
        /// Configures the Swagger generation options. Adds each API version as a separate document and sets up security definitions.
        /// </summary>
        /// <param name="options">The Swagger generation options to configure.</param>
        public void Configure(SwaggerGenOptions options)
        {
            // Documentation for each version
            foreach (ApiVersionDescription description in _provider.ApiVersionDescriptions)
            {
                _logger.LogInformation("CustomSwaggerOptions() | ForEach - Description Api Version: {descriptionGroupName}", description.GroupName);

                options.SwaggerDoc($"api-{description.GroupName}", new OpenApiInfo
                {
                    Title = "Ezenity API",
                    Version = description.ApiVersion.ToString(),
                    Description = "The Ezenity.API serves as a versatile backend for any frontend technology that can plug into it. It offers a range of features from account creation and management to customizable profiles and sections. The API is designed to be flexible, allowing for various use-cases including basic portfolio frontends.",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Email = "anthonymmacallister@gmail.com",
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

            // Documentation for DocInclusionPredicate
            options.DocInclusionPredicate((documentName, apiDescription) =>
            {
                var actionApiVersionModel = apiDescription.ActionDescriptor.GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                _logger.LogInformation("CustomSwaggerOptions() | Doc Inclusion | Params - documentName: {documentName}", documentName);
                _logger.LogInformation("CustomSwaggerOptions() | Doc Inclusion | Params - apiDescription: {apiDescription}", apiDescription);
                _logger.LogInformation("CustomSwaggerOptions() | Doc Inclusion | Variable - Action Api Version Model: {actionApiVersionModel}", actionApiVersionModel);

                if (actionApiVersionModel == null)
                {
                    _logger.LogInformation("CustomSwaggerOptions() | Doc Inclusion | Variable - ActionApiVersionModel is null.");
                    return true;
                }

                _logger.LogInformation("CustomSwaggerOptions() | Doc Inclusion | Variable - actionApiVersionModel.DeclaredApiVersion: {actionApiVersionModel.DeclaredApiVersions}", actionApiVersionModel.DeclaredApiVersions.Any());

                if (actionApiVersionModel.DeclaredApiVersions.Any())
                {
                    // var isIncluded = actionApiVersionModel.DeclaredApiVersions.Any(v => $"ProjectEzenityApi-{v}" == documentName);
                    // var isIncluded = actionApiVersionModel.DeclaredApiVersions.Any(v => string.Equals($"ProjectEzenityApi-{v}", documentName, StringComparison.OrdinalIgnoreCase));
                    var isIncluded = actionApiVersionModel.DeclaredApiVersions.Any(v =>
                    {
                        _logger.LogInformation("CustomSwaggerOptions() | Doc Inclusion | Variable - isIncluded Comparing declared version '{DeclaredVersion}' with document name '{DocumentName}'", v, documentName);
                        return $"api-v{v}" == documentName;
                    });

                    _logger.LogInformation("CustomSwaggerOptions() | Doc Inclusion | Variable - isIncluded: {isIncluded}", isIncluded);
                    _logger.LogInformation("CustomSwaggerOptions() | Doc Inclusion | Variable - actionApiVersionModel.DeclaredApiVersion Selected: {actionApiVersionModel.DeclaredApiVersions}\n", string.Join(", ", actionApiVersionModel.DeclaredApiVersions.Select(v => v.ToString())));

                    return isIncluded;
                }

                var isImplemented = actionApiVersionModel.ImplementedApiVersions.Any(v =>
                {
                    _logger.LogInformation("CustomSwaggerOptions() | Doc Inclusion | Variable - isImplemented Comparing implemented version '{ImplementedVersion}' with document name '{DocumentName}'", v, documentName);
                    return $"api-v{v}" == documentName;
                });
                _logger.LogInformation("CustomSwaggerOptions() | Doc Inclusion | Variable - isImplemented: {isImplemented}\n", isImplemented);
                return isImplemented;
            });

            // Documentation for AddSecurityDefinition
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'. Use this format when making authenticated API calls. You can obtain the JWT Bearer token by making a POST request to http://localhost:5000/api/v1/accounts/authenticate.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                // Type = SecuritySchemeType.Http,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer"
            });

            // Documentation for AddSecurityRequirement
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        Scheme = "bearer",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });

            // Create Operational Filters
            options.OperationFilter<CreateEmailTemplateFilter>();
            options.OperationFilter<CreateSectionFilter>();

            // Update Operational Filters
            options.OperationFilter<UpdateAccountFilter>();

            // Get Operational Filters
            options.OperationFilter<GetEmailTemplateFilter>();

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
