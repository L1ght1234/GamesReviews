using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.AuthMethods;
using GamesReviews.Application.Comments.Features.CreateComment;
using GamesReviews.Application.Comments.Features.DeleteComment;
using GamesReviews.Application.Comments.Features.GetComments;
using GamesReviews.Application.Comments.Features.UpdateComment;
using GamesReviews.Application.Reports.Features.CreateReport;
using GamesReviews.Application.Reports.Features.GetMyReport;
using GamesReviews.Application.Reports.Features.GetReport;
using GamesReviews.Application.Reports.Features.UpdateReportStatus;
using GamesReviews.Application.Reviews.Features.CreateReview;
using GamesReviews.Application.Reviews.Features.DeleteReview;
using GamesReviews.Application.Reviews.Features.GetMyReview;
using GamesReviews.Application.Reviews.Features.GetReview;
using GamesReviews.Application.Reviews.Features.GetReviewById;
using GamesReviews.Application.Reviews.Features.UpdateReview;
using GamesReviews.Application.Users.Features.DeleteUser;
using GamesReviews.Application.Users.Features.GetMyUser;
using GamesReviews.Application.Users.Features.GetUser;
using GamesReviews.Application.Users.Features.Login;
using GamesReviews.Application.Users.Features.Register;
using GamesReviews.Application.Users.Features.RegisterModerator;
using GamesReviews.Application.Users.Features.UpdateUser;
using GamesReviews.Application.Users.Features.UpdateUserForModeration;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Comments;
using GamesReviews.Contracts.Reports;
using GamesReviews.Contracts.Reviews;
using GamesReviews.Contracts.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GamesReviews.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        
        services.AddHttpContextAccessor();
        
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        
        services.AddScoped<ICommandHandler<PagedResult<GetReviewResponse>, GetReviewCommand>, GetReviewHandler>();
        services.AddScoped<ICommandHandler<GetUserResponse, GetMyUserCommand>, GetMyUserHandler>();
        services.AddScoped<ICommandHandler<Guid, CreateReviewCommand>, CreateReviewHandler>();
        services.AddScoped<ICommandHandler<Guid, UpdateReviewCommand>, UpdateReviewHandler>();
        services.AddScoped<ICommandHandler<Guid, DeleteReviewCommand>, DeleteReviewHandler>();
        services.AddScoped<ICommandHandler<GetReviewResponse, GetReviewByIdCommand>, GetReviewByIdHandler>();
        
        services.AddScoped<ICommandHandler<PagedResult<GetReviewResponse>, GetMyReviewCommand>, GetMyReviewHandler>();
        services.AddScoped<ICommandHandler<Guid, RegisterCommand>, RegisterHandler>();
        services.AddScoped<ICommandHandler<string, LoginCommand>, LoginHandler>();
        services.AddScoped<ICommandHandler<RegisterModeratorCommand>, RegisterModeratorHandler>();
        services.AddScoped<ICommandHandler<Guid, UpdateUserCommand>, UpdateUserHandler>();
        services.AddScoped<ICommandHandler<DeleteUserCommand>, DeleteUserHandler>();
        services.AddScoped<ICommandHandler<Guid, UpdateUserForModerationCommand>, UpdateUserForModerationHandler>();
        services.AddScoped<ICommandHandler<PagedResult<GetUserResponse>, GetUserCommand>, GetUserHandler>();
        
        services.AddScoped<ICommandHandler<Guid, CreateCommentCommand>, CreateCommentHandler>();
        services.AddScoped<ICommandHandler<PagedResult<CommentResponse>, GetCommentsCommand>, GetCommentsHandler>();
        services.AddScoped<ICommandHandler<Guid, DeleteCommentCommand>, DeleteCommentHandler>();
        services.AddScoped<ICommandHandler<Guid, UpdateCommentCommand>, UpdateCommentHandler>();
        
        services.AddScoped<ICommandHandler<Guid, CreateReportCommand>, CreateReportHandler>();
        services.AddScoped<ICommandHandler<PagedResult<GetReportResponse>, GetReportCommand>, GetReportHandler>();
        services.AddScoped<ICommandHandler<PagedResult<GetReportResponse>, GetMyReportCommand>, GetMyReportHandler>();
        services.AddScoped<ICommandHandler<Guid, UpdateReportStatusCommand>, UpdateReportStatusHandler>();
        
        return services;
    }
}