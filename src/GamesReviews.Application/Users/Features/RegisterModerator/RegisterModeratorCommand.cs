using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Users;

namespace GamesReviews.Application.Users.Features.RegisterModerator;

public record RegisterModeratorCommand(RegisterUserRequest RegisterUserRequest) : ICommand;