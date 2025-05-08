using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Comments.Excaptions;

public class CommentMismatchException : BadRequestException
{
    public CommentMismatchException(Error[] errors) : base(errors)
    {
    }
}