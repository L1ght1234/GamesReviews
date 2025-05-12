using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Users;

namespace GamesReviews.Application.Users.Features.GetUser;

public record GetUserCommand(GetUserFilterRequest GetUserFilterRequest) : ICommand;