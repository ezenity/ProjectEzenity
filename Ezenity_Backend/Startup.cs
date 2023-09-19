using Ezenity_Backend.Helpers;
using Ezenity_Backend.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Ezenity_Backend.Services.Sections;
using Ezenity_Backend.Services.Emails;
using System;
using Microsoft.AspNetCore.Http.Json;
using Ezenity_Backend.Services.Common;
using Ezenity_Backend.Services.Accounts;
using Ezenity_Backend.Services.EmailTemplates;
using Ezenity_Backend.Filters;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Ezenity_Backend
{
    /// <summary>
    /// Initializes the configuration and services for the application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Application's configuration settings.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Application's configuration settings.</param>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Configures application services and adds them to the dependency injection container.
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(_configuration.GetSection("AllowedOrigins").Get<string[]>()) // Use the origins from the configuration
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

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
                        StatusCodes.Status406NotAcceptable));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(
                        StatusCodes.Status500InternalServerError));
            }).AddXmlDataContractSerializerFormatters();

            // Configure Microsoft Json
            services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                AppConfiguration.ConfigureJsonOptions(options);
            });


            // Requires NewtonsoftJsonOutputFormatter
            /*services.Configure<MvcOptions>(options =>
            {
                var jsonOutputFormatter = options.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();

                if(jsonOutputFormatter != null)
                {
                    // Remove text/json as it ins't the aproved media type
                    // for working with JSON at API level
                    if (jsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
                    {
                        jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
                    }
                }
            });*/



            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("ProjectEzenityAPISpecs", new OpenApiInfo {
                    Title = "Project Ezenity API",
                    Version = "v1",
                    Description = "Through this API you can access accounts, sections, roles, operations, and email templates.",
                    Contact = new()
                    {
                        Email = "anthonymmacallister@gmail.com",
                        Name = "Anthony MacAllister",
                        Url = new Uri("https://ezenity.com/")
                    },
                    License = new()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                c.IncludeXmlComments(xmlCommentsFullPath);
            });

            // Configure stringly typed settings objects
            services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));

            // Make the data repository available for dependency injection. Whenever an interface is 
            // referenced in a constructor, substitute an instace of the class.
            //
            // AddScope: Only one instance of the class is created in a given HTTP request (Last for whole HTTP request)
            // AddTransient: Generate a new instance of the class each time it is requested
            // AddSingleton: Geernate only one clas instance for the lifetime of the whole app
            // configure Dependecy Injection for application services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<IAuthService, AuthService>();

            // Add IConfiguration to the service container
            //services.AddSingleton(_configuration); // Not necessary, as the 'IConfiguration' instance is added to the services container by default by the host.

            string secretKey;

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? System.IO.File.ReadAllText("secrete/file/location").Trim(); // TODO: Insert correct location once on server
            else
                secretKey = _configuration.GetSection("AppSettings")["Secret"];
            
            services.AddSingleton(sp =>
                    new TokenHelper(secretKey));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Filter DI
            services.AddScoped<LoadAccountFilter>();
        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="dataContext">Database context for entity operations.</param>
        public void Configure(IApplicationBuilder app, DataContext dataContext)
        {

            // Migrate any database changes on startup (includes initial db creation)
            // Ensure to be used only for development. For production, run migrations 
            // manually incase of breaking changes in the database schema.
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
                dataContext.Database.Migrate();

            // Generate swagger json and swagger ui middleware
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/ProjectEzenityAPISpecs/swagger.json", "Project Ezenity API");
                x.RoutePrefix = String.Empty;
            });

            /*app.UseRouting();*/

            // Global CORS policy
            app.UseCors("CorsPolicy");

            app.UseRouting();

            // Global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // Custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(x => x.MapControllers());
        }
    }
}
