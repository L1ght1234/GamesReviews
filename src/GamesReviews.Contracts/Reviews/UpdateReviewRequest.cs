namespace GamesReviews.Contracts.Reviews;

public record UpdateReviewRequest(string GameName, string Description, List<string> Tags);