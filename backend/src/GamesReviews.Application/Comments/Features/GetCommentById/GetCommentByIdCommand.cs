using GamesReviews.Application.Abstractions;

namespace GamesReviews.Application.Comments.Features.GetCommentById;

public record GetCommentByIdCommand(Guid CommentId) : ICommand;