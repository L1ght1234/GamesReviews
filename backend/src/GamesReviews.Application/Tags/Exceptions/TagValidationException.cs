using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Tags.Exceptions;

public class TagValidationException : BadRequestException
{
    public TagValidationException(Error[] errors) : base(errors)
    {
    }
}
