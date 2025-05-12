namespace GamesReviews.Contracts.Reviews;

public record GetReviewResponse(
    Guid ReviewId,
    Guid UserId,
    string GameName,
    string Description,
    DateTime CreatedAt,
    List<string> Tags
);