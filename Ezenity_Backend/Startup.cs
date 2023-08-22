using Ezenity_Backend.Helpers;
using Ezenity_Backend.Middleware;
using Ezenity_Backend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Ezenity_Backend.Services.Sections;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ezenity_Backend.Services.Emails;

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

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
                    
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ezenity_Backend", Version = "v1" });
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

            // Add IConfiguration to the service container
            //services.AddSingleton(_configuration); // Not necessary, as the 'IConfiguration' instance is added to the services container by default by the host.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, DataContext dataContext)
        {

            // Migrate any database changes on startup (includes initial db creation)
            // Ensure to be used only for development. For production, run migrations 
            // manually incase of breaking changes in the database schema.
            dataContext.Database.Migrate();

            // Generate swagger json and swagger ui middleware
            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Ezenity_Backend v1"));

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
