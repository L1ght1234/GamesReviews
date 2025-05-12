using GamesReviews.Application.Comments;
using GamesReviews.Application.Reports;
using GamesReviews.Application.Reviews;
using GamesReviews.Application.Tags;
using GamesReviews.Application.Users;
using GamesReviews.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GamesReviews.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GamesReviewsDbContext>(
            options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(nameof(GamesReviewsDbContext)));
            });
        
        services.AddScoped<IReviewsRepository, ReviewsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ITagsRepository, TagsRepository>();
        services.AddScoped<ICommentsRepository, CommentsRepository>();
        services.AddScoped<IReportsRepository, ReportsRepository>();
        
        return services;
    }
}