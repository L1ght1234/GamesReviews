using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Reviews;

namespace GamesReviews.Application.Reviews.Features.GetMyReview;

public record GetMyReviewCommand(GetReviewFilterRequest Request, Guid UserId) : ICommand;