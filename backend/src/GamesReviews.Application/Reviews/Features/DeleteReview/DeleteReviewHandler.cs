using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Exceptions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Reviews.Exceptions;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Reviews.Features.DeleteReview;

public class DeleteReviewHandler : ICommandHandler<Guid, DeleteReviewCommand>
{
    private readonly IReviewsRepository _reviewsRepository;
    private readonly ILogger<DeleteReviewHandler> _logger;
     
    public DeleteReviewHandler(
        IReviewsRepository reviewsRepository,
        ILogger<DeleteReviewHandler> logger)
    {
        _reviewsRepository = reviewsRepository;
        _logger = logger;
    }
    
    public async Task<Guid> Handle(DeleteReviewCommand command, CancellationToken cancellationToken)
    {
        var existingReview = await _reviewsRepository.GetByIdAsync(command.ReviewId, cancellationToken);
        if (existingReview is null)
        {
            throw new ReviewNotFoundException(command.ReviewId.ToNotFoundError("Review not found."));
        }

        var isOwner = existingReview.UserId == command.UserId;
        var isModerator = command.UserRole == "Admin" || command.UserRole == "Moderator";
        
        if (!isOwner && !isModerator)
        {
            throw new ForbiddenActionException("Review".ToForbiddenErrors());
        }

        await _reviewsRepository.DeleteAsync(command.ReviewId, cancellationToken);

        _logger.LogInformation("Deleted review with id {ReviewId} by user {UserId}", command.ReviewId, command.UserId);

        return command.ReviewId;
    }
}