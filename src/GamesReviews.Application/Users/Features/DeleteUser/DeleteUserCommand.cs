using GamesReviews.Application.Abstractions;

namespace GamesReviews.Application.Users.Features.DeleteUser;

public record DeleteUserCommand(Guid UserId) : ICommand;