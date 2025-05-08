using FluentValidation;
using GamesReviews.Contracts.Users;

namespace GamesReviews.Application.Users.Features.UpdateUser;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    private readonly IUsersRepository _usersRepository;

    public UpdateUserRequestValidator(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
        
        RuleFor(x => x.UserName)
            .MaximumLength(20).WithMessage("UserName name must not exceed 20 characters.")
            .MustAsync(BeUniqueUsername).WithMessage("Username already taken");
        
        RuleFor(x => x.CurrentPassword)
            .MaximumLength(30).WithMessage("Password must not exceed 30 characters.");
        
        RuleFor(x => x.NewPassword)
            .MaximumLength(30).WithMessage("Password must not exceed 30 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(BeUniqueEmail).WithMessage("Email already exists");
    }
    private async Task<bool> BeUniqueEmail(UpdateUserRequest request, string email, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmailAsync(email, cancellationToken);
        return user == null || user.Id == request.UserId;
    }

    private async Task<bool> BeUniqueUsername(UpdateUserRequest request, string username, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByUserNameAsync(username, cancellationToken);
        return user == null || user.Id == request.UserId;
    }
}