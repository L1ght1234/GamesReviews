using GamesReviews.Application;
using GamesReviews.Application.AuthMethods;
using GamesReviews.Extensions;
using GamesReviews.Infrastructure;
using Microsoft.Extensions.Options;

namespace GamesReviews;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddWebDependencies();
        
        services.AddApplication(configuration);

        services.AddInfrastructure(configuration);
        
        services.AddApiAuth(
            services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>());
        
        return services;
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();
        
        return services;
    }
}