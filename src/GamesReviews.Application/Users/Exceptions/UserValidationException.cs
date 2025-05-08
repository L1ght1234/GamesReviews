using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Users.Exceptions;

public class UserValidationException : BadRequestException
{
    public UserValidationException(Error[] errors) : base(errors)
    {
    }
}
