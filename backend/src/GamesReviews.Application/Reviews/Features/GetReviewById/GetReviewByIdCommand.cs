using GamesReviews.Application.Abstractions;

namespace GamesReviews.Application.Reviews.Features.GetReviewById;

public record GetReviewByIdCommand(Guid ReviewId) :ICommand;