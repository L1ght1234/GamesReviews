using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Users;

namespace GamesReviews.Application.Users.Features.UpdateUserForModeration;

public record UpdateUserForModerationCommand(Guid UserId, UpdateUserRequestForModeration UpdateUserRequestForModeration) : ICommand;