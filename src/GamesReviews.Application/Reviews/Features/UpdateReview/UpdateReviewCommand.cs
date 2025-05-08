using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Reviews;

namespace GamesReviews.Application.Reviews.Features.UpdateReview;

public record UpdateReviewCommand(Guid ReviewId, Guid UserId, string UserRole, UpdateReviewRequest UpdateReviewRequest) : ICommand;