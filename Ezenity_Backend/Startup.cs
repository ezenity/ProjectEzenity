using Ezenity_Backend.Helpers;
using Ezenity_Backend.Middleware;
using Ezenity_Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity_Backend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the (DI) container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>();
            services.AddCors();
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ezenity_Backend", Version = "v1" });
            });

            // Configure stringly typed settings objects
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // Make the data repository available for dependency injection. Whenever an interface is 
            // referenced in a constructor, substitute an instace of the class.
            //
            // AddScope: Only one instance of the class is created in a given HTTP request (Last for whole HTTP request)
            // AddTransient: Generate a new instance of the class each time it is requested
            // AddSingleton: Geernate only one clas instance for the lifetime of the whole app
            // configure Dependecy Injection for application services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IEmailService, EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, DataContext dataContext)
        {

            // Migrate any database changes on startup (includes initial db creation)
            dataContext.Database.Migrate();

            // Generate swagger json and swagger ui middleware
            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Ezenity_Backend v1"));

            app.UseRouting();

            // Global CORS policy
            app.UseCors(x => x
                    .SetIsOriginAllowed(origin => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());

            // Global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // Custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(x => x.MapControllers());
        }
    }
}
