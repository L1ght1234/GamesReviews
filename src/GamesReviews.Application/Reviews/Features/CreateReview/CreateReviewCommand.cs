using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Reviews;

namespace GamesReviews.Application.Reviews.Features.CreateReview;

public record CreateReviewCommand(Guid UserId, CreateReviewRequest CreateReviewRequest) : ICommand;