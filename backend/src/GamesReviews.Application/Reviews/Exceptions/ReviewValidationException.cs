using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Reviews.Exceptions;

public class ReviewValidationException : BadRequestException
{
    public ReviewValidationException(Error[] errors) : base(errors)
    {
    }
}
