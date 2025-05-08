using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.AuthMethods;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Users.Exceptions;
using GamesReviews.Contracts.Users;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Users.Features.UpdateUser;

public class UpdateUserHandler : ICommandHandler<Guid, UpdateUserCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<UpdateUserRequest> _validator;
    private readonly ILogger<UpdateUserHandler> _logger;

    public UpdateUserHandler(
        IPasswordHasher passwordHasher, 
        IUsersRepository usersRepository, 
        IValidator<UpdateUserRequest> validator, 
        ILogger<UpdateUserHandler> logger)
    {
        _passwordHasher = passwordHasher;
        _usersRepository = usersRepository;
        _validator = validator;
        _logger = logger;
    }
    public async Task<Guid> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.UpdateUserRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new UserValidationException(validationResult.ToErrors());
        }

        var existingUser = await _usersRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (existingUser == null)
        {
            throw new UserNotFoundException(command.UserId.ToNotFoundError("User not found."));
        }
        
        var result = _passwordHasher.Verify(command.UpdateUserRequest.CurrentPassword, existingUser.HashedPassword);
        
        if (!result)
            throw new InvalidCredentialsException();
        
        var passwordHash = _passwordHasher.Generate(command.UpdateUserRequest.NewPassword);
        
        existingUser.Update(
            newUserName: command.UpdateUserRequest.UserName,
            newHashedPassword: passwordHash,
            newEmail: command.UpdateUserRequest.Email);

        await _usersRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Updated user with id {UserId}", command.UserId);

        return command.UserId;
    }
}