namespace GamesReviews.Contracts.Users;

public record GetUserFilterRequest(
    string? Search = null,
    string? Role = null,
    string? SortBy = "UserName",
    string? SortDirection = "asc",
    int Page = 1,
    int PageSize = 10
);