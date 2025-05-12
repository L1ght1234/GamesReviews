using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Users;

namespace GamesReviews.Application.Users.Features.UpdateUser;

public record UpdateUserCommand(Guid UserId, UpdateUserRequest UpdateUserRequest) : ICommand;