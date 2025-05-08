using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Comments.Excaptions;

public class CommentValidationException : BadRequestException
{
    public CommentValidationException(Error[] errors) : base(errors)
    {
    }
}