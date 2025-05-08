using GamesReviews.Application.Abstractions;

namespace GamesReviews.Application.Users.Features.GetMyUser;

public record GetMyUserCommand(Guid UserId) : ICommand;