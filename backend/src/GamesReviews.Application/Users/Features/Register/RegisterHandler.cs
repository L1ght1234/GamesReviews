using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.AuthMethods;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Users.Exceptions;
using GamesReviews.Contracts.Users;
using GamesReviews.Domain.Users;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Users.Features.Register;

public class RegisterHandler : ICommandHandler<Guid, RegisterCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterUserRequest> _validator;
    private readonly ILogger<RegisterHandler> _logger;

    public RegisterHandler(
        IPasswordHasher passwordHasher, 
        IUsersRepository usersRepository, 
        IValidator<RegisterUserRequest> validator, 
        ILogger<RegisterHandler> logger)
    {
        _passwordHasher = passwordHasher;
        _usersRepository = usersRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Guid> Handle(RegisterCommand command, CancellationToken cancellationToken)
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
            role: "User");

        if (errors.Any())
        {
            throw new UserValidationException(errors.ToValidationErrors());
        }
        
        await _usersRepository.AddAsync(user!, cancellationToken);
        
        _logger.LogInformation("Created user with id {UserId}", user!.Id);
        
        return user.Id;
    }
}