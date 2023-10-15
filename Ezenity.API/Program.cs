using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Linq;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.StaticFiles;
using Serilog;
using Ezenity.API.Filters;
using Ezenity.Infrastructure.Helpers;
using Ezenity.Core.Services.Common;
using Ezenity.Infrastructure.Services.Accounts;
using Ezenity.Infrastructure.Services.Emails;
using Ezenity.Infrastructure.Services.EmailTemplates;
using Ezenity.Infrastructure.Services.Sections;
using Ezenity.API.Middleware;
using Ezenity.Core.Interfaces;
using Microsoft.Extensions.Options;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/api.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory()
});

var environmentName = builder.Environment.EnvironmentName;

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
    environmentName = Environments.Development;
else
    environmentName = Environments.Production;

//builder.Logging.ClearProviders();
/*builder.Logging.AddConfiguration(configuration.GetSection("Logging"));
builder.Logging.AddJsonConsole();
builder.Logging.AddConsole();
builder.Logging.AddDebug();*/

// Set Logging Provider
builder.Host.UseSerilog();

Console.WriteLine("###################################");
Console.WriteLine($"Application Name: {builder.Environment.ApplicationName}");
Console.WriteLine($"Environment Name: {builder.Environment.EnvironmentName}");
Console.WriteLine($"ContentRoot Path: {builder.Environment.ContentRootPath}");
Console.WriteLine("###################################");

// Application's configuration settings
var configuration = builder.Configuration;

// Add configurations from appsettings.json, appsettings.Development.json, etc.
configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

// Add serices to the container
var services = builder.Services;

services.AddControllers(options =>
{
    // This will return a 406 when the return type is not acceptable
    options.ReturnHttpNotAcceptable = true;

    options.Filters.Add<ApiExceptionFilter>();
    // options.Filters.Add(typeof(LoadAccountFilter)); // Registers this filter globally
    /*options.Filters.Add(
        new ProducesResponseTypeAttribute(
            StatusCodes.Status200OK));*/
    options.Filters.Add(
        new ProducesResponseTypeAttribute(
            StatusCodes.Status400BadRequest));
    options.Filters.Add(
        new ProducesResponseTypeAttribute(
            StatusCodes.Status401Unauthorized));
    options.Filters.Add(
        new ProducesResponseTypeAttribute(
            StatusCodes.Status406NotAcceptable));
    options.Filters.Add(
        new ProducesResponseTypeAttribute(
            StatusCodes.Status500InternalServerError));
}).AddNewtonsoftJson(setupAction =>
{
    setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    setupAction.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
    setupAction.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
}).AddXmlDataContractSerializerFormatters();

services.Configure<MvcOptions>(options =>
{
    var jsonOutputFormatter = options.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();

    if (jsonOutputFormatter != null)
    {
        // Remove text/json as it ins't the aproved media type
        // for working with JSON at API level
        if (jsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
        {
            jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
        }
    }
});

// Configure stringly typed settings objects
services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
services.AddSingleton<IAppSettings, AppSettingsWrapper>();

// TODO: Add support for Azure Kay Vault
var connectionString = configuration.GetConnectionString("WebApiDatabase");
services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

// Make the data repository available for dependency injection. Whenever an interface is 
// referenced in a constructor, substitute an instace of the class.
//
// AddScope: Only one instance of the class is created in a given HTTP request (Last for whole HTTP request)
// AddTransient: Generate a new instance of the class each time it is requested
//      Good for lightwieght stateless services
// AddSingleton: Geernate only one class instance for the lifetime of the whole app
// configure Dependecy Injection for application services
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

string secretKey;
if (builder.Environment.IsDevelopment())
    secretKey = configuration.GetSection("AppSettings")["Secret"];
else
    secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? System.IO.File.ReadAllText("./secret/key.txt").Trim(); // TODO: Insert correct location once on server

// Register AppSettings with the updated secretKey
var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
appSettings.Secret = secretKey;

services.AddSingleton(appSettings);
services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()) // Use the origins from the configuration
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

if (builder.Environment.IsDevelopment())
{
    services.AddApiVersioning(setupAction =>
    {
        setupAction.AssumeDefaultVersionWhenUnspecified = true;
        setupAction.DefaultApiVersion = new ApiVersion(1, 0);
        setupAction.ReportApiVersions = true;
    //setupAction.ApiVersionReader = new HeaderApiVersionReader("api-version");
    //setupAction.ApiVersionReader = new MediaTypeApiVersionReader();
    });

    services.AddVersionedApiExplorer(setupAction =>
    {
        setupAction.GroupNameFormat = "'v'VV";
    });

    var apiVersionDescriptionProvider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
    var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

    services.AddSwaggerGen(setupAction =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            setupAction.SwaggerDoc($"ProjectEzenityAPISpecs{description.GroupName}", new OpenApiInfo
            {
                Title = "Project Ezenity API",
                Version = description.ApiVersion.ToString(),
                Description = "Through this API you can access accounts, sections, roles, operations, and email templates.",
                Contact = new()
                {
                    Email = "anthonymmacallister@gmail.com",
                    Name = "Anthony MacAllister",
                    Url = new Uri("https://Ezenity.com/")
                },
                License = new()
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });
        }

        setupAction.DocInclusionPredicate((documentName, apiDescription) =>
        {
            var actionApiVersionModel = apiDescription.ActionDescriptor.GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

            logger.LogInformation($"Evaluating API description for document: {documentName}");
            logger.LogInformation($"ActionApiVersionModel: {actionApiVersionModel}");

            if (actionApiVersionModel == null)
            {
                logger.LogInformation("ActionApiVersionModel is null. Including in document.");
                return true;
            }

            if (actionApiVersionModel.DeclaredApiVersions.Any())
            {
            //var isIncluded = actionApiVersionModel.DeclaredApiVersions.Any(v => $"ProjectEzenityAPISpecs{v}" == documentName);
            var isIncluded = actionApiVersionModel.DeclaredApiVersions.Any(v => string.Equals($"ProjectEzenityAPISpecsv{v}", documentName, StringComparison.OrdinalIgnoreCase));

                logger.LogInformation($"Is included in DeclaredApiVersions: {isIncluded}");

                logger.LogInformation($"Document Name: {documentName}");
                logger.LogInformation($"Declared API Versions: {string.Join(", ", actionApiVersionModel.DeclaredApiVersions.Select(v => v.ToString()))}");

                return isIncluded;
            }

            var isImplemented = actionApiVersionModel.ImplementedApiVersions.Any(v => $"ProjectEzenityAPISpecsv{v}" == documentName);
            logger.LogInformation($"Is included in ImplementedApiVersions: {isImplemented}");
            return isImplemented;
        });

        setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'. Use this format when making authenticated API calls. You can obtain the JWT Bearer token by making a POST request to http://localhost:5000/api/v1/accounts/authenticate.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });

        setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] { }
            }
        });

        // Create Operational Filters
        setupAction.OperationFilter<CreateEmailTemplateFilter>();
        setupAction.OperationFilter<CreateSectionFilter>();

        // Update Operational Filters
        setupAction.OperationFilter<UpdateAccountFilter>();

        // Get Operational Filters
        setupAction.OperationFilter<GetEmailTemplateFilter>();

        var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

        setupAction.IncludeXmlComments(xmlCommentsFullPath);
    });
}

services.AddSingleton<FileExtensionContentTypeProvider>();


services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "CustomJwt";
    options.DefaultChallengeScheme = "CustomJwt";
}).AddScheme<AuthenticationSchemeOptions, CustomJwtAuthenticationHandler>("CustomJwt", null);

services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});



// Build the application
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseStaticFiles();

    // Generate swagger json and swagger ui middleware
    app.UseSwagger();
    app.UseSwaggerUI(setupAction =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            setupAction.SwaggerEndpoint(
                $"/swagger/ProjectEzenityAPISpecs{description.GroupName}/swagger.json",
                $"Project Ezenity API {description.GroupName.ToUpperInvariant()}");
        }

        setupAction.DefaultModelExpandDepth(2);
        setupAction.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
        setupAction.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Default = List
        setupAction.EnableDeepLinking();
        setupAction.DisplayOperationId();

        setupAction.InjectStylesheet("/assets/custom-ui.css");

        setupAction.RoutePrefix = String.Empty;
    });
}

// Global CORS policy
app.UseCors("CorsPolicy");

app.UseRouting();

// Global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

// Custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

if(app.Environment.IsProduction())
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(
    endpoints => endpoints.MapControllers()
);

// Migrate any database changes on startup (includes initial db creation)
// Ensure to be used only for development. For production, run migrations 
// manually incase of breaking changes in the database schema.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();
}

app.Run();