using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Tags.Exceptions;

public class TagNotFoundException : NotFoundException
{
    public TagNotFoundException(Error[] errors) : base(errors)
    {
    }
}