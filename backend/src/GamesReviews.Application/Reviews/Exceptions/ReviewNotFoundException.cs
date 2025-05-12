using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Reviews.Exceptions;

public class ReviewNotFoundException : NotFoundException
{
    public ReviewNotFoundException(Error[] errors) : base(errors)
    {
    }
}