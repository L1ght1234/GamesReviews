namespace GamesReviews.Contracts.Comments;

public record GetCommentsRequest(
    int Page = 1,
    int PageSize = 10);