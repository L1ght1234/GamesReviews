using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Comments.Excaptions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Reviews;
using GamesReviews.Application.Reviews.Exceptions;
using GamesReviews.Contracts.Comments;
using GamesReviews.Domain.Comments;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Comments.Features.CreateComment;

public class CreateCommentHandler : ICommandHandler<Guid, CreateCommentCommand>
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly IReviewsRepository _reviewsRepository;
    private readonly IValidator<CreateCommentRequest> _validator;
    private readonly ILogger<CreateCommentHandler> _logger;

    public CreateCommentHandler(
        ICommentsRepository commentsRepository,
        IReviewsRepository reviewsRepository,
        IValidator<CreateCommentRequest> validator, 
        ILogger<CreateCommentHandler> logger)
    {
        _commentsRepository = commentsRepository;
        _reviewsRepository = reviewsRepository;
        _validator = validator;
        _logger = logger;
    }
    public async Task<Guid> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.CreateCommentRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new CommentValidationException(validationResult.ToErrors());
        }

        var review = await _reviewsRepository.GetByIdAsync(command.ReviewId, cancellationToken);
        if (review is null)
            throw new ReviewNotFoundException(command.ReviewId.ToNotFoundError("Review not found"));

        if (command.ParentCommentId is not null)
        {
            var parent = await _commentsRepository.GetByIdAsync(command.ParentCommentId.Value, cancellationToken);
            if (parent is null)
                throw new CommentNotFoundException(command.ParentCommentId.Value.ToNotFoundError("Comment"));
            
            if (parent.ReviewId != command.ReviewId)
                throw new CommentMismatchException("Comment".ToMismatchErrors());        
        }

        var (comment, errors) = Comment.Create(
            id: Guid.NewGuid(),
            userId: command.UserId,
            reviewId: command.ReviewId,
            text: command.CreateCommentRequest.Text,
            parentCommentId: command.ParentCommentId
        );

        if (errors.Any())
            throw new CommentValidationException(errors.ToValidationErrors());

        await _commentsRepository.AddAsync(comment!, cancellationToken);

        _logger.LogInformation("User {UserId} created comment {CommentId} on review {ReviewId}{ParentInfo}",
            command.UserId,
            comment!.Id,
            command.ReviewId,
            command.ParentCommentId is not null ? $" as reply to {command.ParentCommentId}" : "");
        
        return comment!.Id;
    }
}