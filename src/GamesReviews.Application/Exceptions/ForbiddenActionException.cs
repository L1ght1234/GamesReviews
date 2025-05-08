using Shared;

namespace GamesReviews.Application.Exceptions;

public class ForbiddenActionException : BadRequestException
{
    public ForbiddenActionException(Error[] errors) : base(errors) { }
}