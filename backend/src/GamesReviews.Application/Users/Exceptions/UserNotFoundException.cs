using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Users.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Error[] errors) : base(errors)
    {
    }
}