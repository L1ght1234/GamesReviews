using GamesReviews.Application.Abstractions;

namespace GamesReviews.Application.Reviews.Features.DeleteReview;

public record DeleteReviewCommand(Guid ReviewId, Guid UserId, string UserRole) : ICommand;