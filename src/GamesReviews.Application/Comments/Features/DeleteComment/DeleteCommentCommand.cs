using GamesReviews.Application.Abstractions;

namespace GamesReviews.Application.Comments.Features.DeleteComment;

public record DeleteCommentCommand(Guid ReviewId, Guid CommentId, Guid UserId, string UserRole) : ICommand;