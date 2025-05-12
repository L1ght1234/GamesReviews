using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Comments.Excaptions;

public class CommentNotFoundException : NotFoundException
{
    public CommentNotFoundException(Error[] errors) : base(errors)
    {
    }
}