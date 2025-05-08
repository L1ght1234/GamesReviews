using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Users.Exceptions;

public class InvalidCredentialsException : BadRequestException
{
    public InvalidCredentialsException() 
        : base(new[] { Error.Validation("login.invalid.credentials", 
            "Invalid email or password", "password") }) { }
}