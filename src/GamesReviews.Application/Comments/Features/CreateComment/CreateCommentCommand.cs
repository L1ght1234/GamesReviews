using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Comments;

namespace GamesReviews.Application.Comments.Features.CreateComment;

public record CreateCommentCommand(Guid UserId, Guid ReviewId, CreateCommentRequest CreateCommentRequest, 
    Guid? ParentCommentId = null) : ICommand;