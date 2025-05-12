using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Reviews;

namespace GamesReviews.Application.Reviews.Features.GetReview;

public record GetReviewCommand(GetReviewFilterRequest GetReviewFilterRequest) : ICommand;