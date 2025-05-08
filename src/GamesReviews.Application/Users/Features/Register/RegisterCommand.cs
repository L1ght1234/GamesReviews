using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Users;

namespace GamesReviews.Application.Users.Features.Register;

public record RegisterCommand(RegisterUserRequest RegisterUserRequest) : ICommand;