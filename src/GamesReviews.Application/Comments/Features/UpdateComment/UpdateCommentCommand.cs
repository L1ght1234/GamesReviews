using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Comments;

namespace GamesReviews.Application.Comments.Features.UpdateComment;

public record UpdateCommentCommand(Guid ReviewId, Guid CommentId, Guid UserId, string UserRole, UpdateCommentRequest UpdateCommentRequest) : ICommand;