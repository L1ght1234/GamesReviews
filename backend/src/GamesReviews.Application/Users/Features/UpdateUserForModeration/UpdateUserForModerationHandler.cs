using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.AuthMethods;
using GamesReviews.Application.Exceptions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Users.Exceptions;
using GamesReviews.Contracts.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Users.Features.UpdateUserForModeration;

public class UpdateUserForModerationHandler : ICommandHandler<Guid, UpdateUserForModerationCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IValidator<UpdateUserRequestForModeration> _validator;
    private readonly ILogger<UpdateUserForModerationHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateUserForModerationHandler(
        IUsersRepository usersRepository, 
        IValidator<UpdateUserRequestForModeration> validator, 
        ILogger<UpdateUserForModerationHandler> logger, 
        IHttpContextAccessor httpContextAccessor, 
        IPasswordHasher passwordHasher)
    {
        _usersRepository = usersRepository;
        _validator = validator;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(UpdateUserForModerationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.UpdateUserRequestForModeration, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new UserValidationException(validationResult.ToErrors());
        }

        var existingUser = await _usersRepository.GetByIdAsync(command.UserId, cancellationToken);
        
        if (existingUser == null)
        {
            throw new UserNotFoundException(command.UserId.ToNotFoundError("User"));
        };
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (existingUser.Role == "Admin")
            throw new ForbiddenActionException(existingUser.Role.ToForbiddenErrors("User"));
        
        if (existingUser.Role == "Moderator")
        {
            if(!httpContext!.User.IsInRole("Admin"))
                throw new ForbiddenActionException(existingUser.Role.ToForbiddenErrors("User"));
        }
        
        var passwordHash = _passwordHasher.Generate(command.UpdateUserRequestForModeration.NewPassword);

        existingUser.Update(
            newUserName: command.UpdateUserRequestForModeration.UserName,
            newHashedPassword: passwordHash,
            newEmail: command.UpdateUserRequestForModeration.Email);

        await _usersRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Updated user with id {UserId}", command.UserId);

        return command.UserId;
    }
}