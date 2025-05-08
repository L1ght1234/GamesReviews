namespace GamesReviews.Contracts.Users;

public record RegisterUserRequest(string UserName, string Email, string Password);