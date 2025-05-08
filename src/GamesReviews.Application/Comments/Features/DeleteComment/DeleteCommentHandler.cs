using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Comments.Excaptions;
using GamesReviews.Application.Exceptions;
using GamesReviews.Application.Extensions;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Comments.Features.DeleteComment;

public class DeleteCommentHandler : ICommandHandler<Guid, DeleteCommentCommand>
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly ILogger<DeleteCommentHandler> _logger;

    public DeleteCommentHandler(
        ICommentsRepository commentsRepository, 
        ILogger<DeleteCommentHandler> logger)
    {
        _commentsRepository = commentsRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        var existingComment = await _commentsRepository.GetByIdAsync(command.CommentId, cancellationToken);
        if (existingComment is null)
        {
            throw new CommentNotFoundException(command.CommentId.ToNotFoundError("Review not found."));
        }

        if (existingComment.ReviewId != command.ReviewId)
            throw new CommentMismatchException("Comment".ToMismatchErrors());        
        
        var isOwner = existingComment.UserId == command.UserId;
        var isModerator = command.UserRole == "Admin" || command.UserRole == "Moderator";
        
        if (!isOwner && !isModerator)
        {
            throw new ForbiddenActionException("Comment".ToForbiddenErrors());        }

        await _commentsRepository.DeleteAsync(command.CommentId, cancellationToken);

        _logger.LogInformation("Deleted comment with id {CommentId} by user {UserId}", command.CommentId, command.UserId);

        return command.CommentId;
    }
}