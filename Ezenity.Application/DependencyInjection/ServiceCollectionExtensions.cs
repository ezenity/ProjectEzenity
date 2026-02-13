using Microsoft.Extensions.DependencyInjection;

namespace Ezenity.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Application-layer workflow services here (when you move implementations)
        // services.AddScoped<IAuthService, AuthService>();
        // services.AddScoped<IAccountService, AccountService>();
        // services.AddScoped<ISectionService, SectionService>();

        return services;
    }
}
