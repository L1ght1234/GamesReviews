using FluentValidation;
using GamesReviews.Contracts.Users;

namespace GamesReviews.Application.Users.Features.Login;

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        
        RuleFor(x => x.Password)
            .MaximumLength(30).WithMessage("Password must not exceed 30 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format");
    }
}