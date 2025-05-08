using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.AuthMethods;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Users.Exceptions;
using GamesReviews.Contracts.Users;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Users.Features.Login;

public class LoginHandler : ICommandHandler<string, LoginCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IValidator<LoginUserRequest> _validator;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        IUsersRepository usersRepository, 
        IPasswordHasher passwordHasher, 
        IJwtProvider jwtProvider, 
        IValidator<LoginUserRequest> validator, ILogger<LoginHandler> logger)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _validator = validator;
        _logger = logger;
    }

    public async Task<string> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.LoginUserRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new UserValidationException(validationResult.ToErrors());
        }
        
        var user = await _usersRepository.GetByEmailAsync(command.LoginUserRequest.Email, cancellationToken);
        if (user is null)
            throw new UserNotFoundException(command.LoginUserRequest.Email.ToNotFoundError("User"));

        var result = _passwordHasher.Verify(command.LoginUserRequest.Password, user!.HashedPassword);
        
        if (!result)
            throw new InvalidCredentialsException();
        
        var token = _jwtProvider.CreateToken(user);
        
        _logger.LogInformation("User with id {UserId} logged in", user!.Id);
        
        return token;
    }
}