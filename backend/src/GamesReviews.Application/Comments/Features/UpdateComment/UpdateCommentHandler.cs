using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Comments.Excaptions;
using GamesReviews.Application.Exceptions;
using GamesReviews.Application.Extensions;
using GamesReviews.Contracts.Comments;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Comments.Features.UpdateComment;

public class UpdateCommentHandler : ICommandHandler<Guid, UpdateCommentCommand>
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly IValidator<UpdateCommentRequest> _validator;
    private readonly ILogger<UpdateCommentHandler> _logger;

    public UpdateCommentHandler(
        ICommentsRepository commentsRepository, 
        IValidator<UpdateCommentRequest> validator, 
        ILogger<UpdateCommentHandler> logger)
    {
        _commentsRepository = commentsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Guid> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.UpdateCommentRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new CommentValidationException(validationResult.ToErrors());
        }

        var existingComment = await _commentsRepository.GetByIdAsync(command.CommentId, cancellationToken);
        if (existingComment == null)
            throw new CommentNotFoundException(command.CommentId.ToNotFoundError("Comment not found."));
        
        if (existingComment.ReviewId != command.ReviewId)
            throw new CommentMismatchException("Comment".ToMismatchErrors());
        
        var isOwner = existingComment.UserId == command.UserId;
        var isModerator = command.UserRole == "Admin" || command.UserRole == "Moderator";
        
        if (!isOwner && !isModerator)
        {
            throw new ForbiddenActionException("Review".ToForbiddenErrors());
        }

        existingComment.Update(
            text: command.UpdateCommentRequest.Text
        );

        await _commentsRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated comment with id: {CommentId} by user {UserId}", command.CommentId, command.UserId);
        return command.CommentId;
    }
}