using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Users;

namespace GamesReviews.Application.Users.Features.Login;

public record LoginCommand(LoginUserRequest LoginUserRequest) : ICommand;