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

namespace Ezenity_Backend
{
    public class Startup
    {

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the (DI) container.
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
                options.Filters.Add<ApiExceptionFilter>();
                // options.Filters.Add(typeof(LoadAccountFilter)); // Registers this filter globally
            });

            services.Configure<JsonOptions>(options =>
            {
                AppConfiguration.ConfigureJsonOptions(options);
            });

            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Project Ezenity API", Version = "v1" });
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, DataContext dataContext)
        {

            // Migrate any database changes on startup (includes initial db creation)
            // Ensure to be used only for development. For production, run migrations 
            // manually incase of breaking changes in the database schema.
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
                dataContext.Database.Migrate();

            // Generate swagger json and swagger ui middleware
            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Ezenity API v1"));

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
