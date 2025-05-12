using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.AuthMethods;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Users.Exceptions;
using GamesReviews.Contracts.Users;
using GamesReviews.Domain.Users;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Users.Features.RegisterModerator;

public class RegisterModeratorHandler: ICommandHandler<RegisterModeratorCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterUserRequest> _validator;
    private readonly ILogger<RegisterModeratorHandler> _logger;

    public RegisterModeratorHandler(
        IPasswordHasher passwordHasher, 
        IUsersRepository usersRepository, 
        IValidator<RegisterUserRequest> validator, 
        ILogger<RegisterModeratorHandler> logger)
    {
        _passwordHasher = passwordHasher;
        _usersRepository = usersRepository;
        _validator = validator;
        _logger = logger;
    }
    public async Task Handle(RegisterModeratorCommand command, CancellationToken cancellationToken)
    {
        var hashedPassword = _passwordHasher.Generate(command.RegisterUserRequest.Password);
        
        var validationResult = await _validator.ValidateAsync(command.RegisterUserRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new UserValidationException(validationResult.ToErrors());
        }
        
        var (user, errors) = User.Create(
            Guid.NewGuid(),
            command.RegisterUserRequest.UserName,
            hashedPassword,
            command.RegisterUserRequest.Email,
            role: "Moderator");

        if (errors.Any())
        {
            throw new UserValidationException(errors.ToValidationErrors());
        }
        
        await _usersRepository.AddAsync(user!, cancellationToken);
        
        _logger.LogInformation("Created moderator with id {UserId}", user!.Id);
    }
}