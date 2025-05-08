using FluentValidation;
using GamesReviews.Contracts.Users;

namespace GamesReviews.Application.Users.Features.Register;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    private readonly IUsersRepository _userRepository;

    public RegisterUserRequestValidator(IUsersRepository userRepository)
    {
        _userRepository = userRepository;
        
        RuleFor(x => x.UserName)
            .MaximumLength(20).WithMessage("UserName name must not exceed 20 characters.")
            .MustAsync(BeUniqueUsername).WithMessage("Username already taken");
        
        RuleFor(x => x.Password)
            .MaximumLength(30).WithMessage("Password must not exceed 30 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(BeUniqueEmail).WithMessage("Email already exists");
    }
    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken) 
        => !await _userRepository.ExistsByEmailAsync(email, cancellationToken);

    private async Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken) 
        => !await _userRepository.ExistsByUsernameAsync(username, cancellationToken);
}