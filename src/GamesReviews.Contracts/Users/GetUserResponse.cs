namespace GamesReviews.Contracts.Users;

public record GetUserResponse(Guid Id,
    string UserName,
    string Email,
    string Role
);