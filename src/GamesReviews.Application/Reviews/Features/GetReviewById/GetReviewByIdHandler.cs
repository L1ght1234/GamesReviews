using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Reviews.Exceptions;
using GamesReviews.Contracts.Reviews;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Reviews.Features.GetReviewById;

public class GetReviewByIdHandler : ICommandHandler<GetReviewResponse, GetReviewByIdCommand>
{
    private readonly IReviewsRepository _reviewsRepository;
    private readonly ILogger<GetReviewByIdHandler> _logger;

    public GetReviewByIdHandler(
        IReviewsRepository reviewsRepository, 
        ILogger<GetReviewByIdHandler> logger)
    {
        _reviewsRepository = reviewsRepository;
        _logger = logger;
    }

    public async Task<GetReviewResponse> Handle(GetReviewByIdCommand command, CancellationToken cancellationToken)
    {
        var review = await _reviewsRepository.GetByIdAsync(command.ReviewId, cancellationToken);
        
        if (review == null)
            throw new ReviewNotFoundException(command.ReviewId.ToNotFoundError("Review not found."));
        
        _logger.LogInformation("Retrieved review with id: {ReviewId}", command.ReviewId);

        return new GetReviewResponse(
            review.Id,
            review.UserId,
            review.GameName,
            review.Description,
            review.CreatedAt,
            review.ReviewTags.Select(rt => rt.Tag.Name).ToList()
        );
    }
}