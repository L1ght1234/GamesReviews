using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Users.Exceptions;
using GamesReviews.Contracts.Users;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Users.Features.GetMyUser;

public class GetMyUserHandler : ICommandHandler<GetUserResponse, GetMyUserCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILogger<GetMyUserHandler> _logger;

    public GetMyUserHandler(IUsersRepository usersRepository,
        ILogger<GetMyUserHandler> logger)
    {
        _usersRepository = usersRepository;
        _logger = logger;
    }

    public async Task<GetUserResponse> Handle(GetMyUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user == null)
            throw new UserNotFoundException(command.UserId.ToNotFoundError("User not found."));

        _logger.LogInformation("User with id {UserId} retrieved their profile", user.Id);
        
        return new GetUserResponse(
            user.Id,
            user.UserName,
            user.Email,
            user.Role);
    }
}