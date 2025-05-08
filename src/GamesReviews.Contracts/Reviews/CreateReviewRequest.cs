namespace GamesReviews.Contracts.Reviews;

public record CreateReviewRequest(
    string GameName,
    string Description,
    List<string> Tags);