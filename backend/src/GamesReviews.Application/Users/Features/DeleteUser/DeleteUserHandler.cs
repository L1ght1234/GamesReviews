using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Exceptions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Users.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Users.Features.DeleteUser;

public class DeleteUserHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILogger<DeleteUserHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteUserHandler(IUsersRepository usersRepository, ILogger<DeleteUserHandler> logger, IHttpContextAccessor httpContextAccessor)
    {
        _usersRepository = usersRepository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await _usersRepository.GetByIdAsync(command.UserId, cancellationToken);
        
        if (existingUser == null)
        {
            throw new UserNotFoundException(command.UserId.ToNotFoundError("User"));
        }
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (existingUser.Role == "Admin")
            throw new ForbiddenActionException(existingUser.Role.ToForbiddenErrors("User"));

        if (existingUser.Role == "Moderator")
        {
            if(!httpContext!.User.IsInRole("Admin"))
                throw new ForbiddenActionException(existingUser.Role.ToForbiddenErrors("User"));
        }

        await _usersRepository.DeleteAsync(command.UserId, cancellationToken);
        _logger.LogInformation("Deleted user with id {UserId}", command.UserId);
    }
}