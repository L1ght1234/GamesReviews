using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Reviews.Exceptions;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Reviews;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Reviews.Features.GetMyReview;

public class GetMyReviewHandler : ICommandHandler<PagedResult<GetReviewResponse>, GetMyReviewCommand>
{
    private readonly IValidator<GetReviewFilterRequest> _validator;
    private readonly IReviewsRepository _reviewsRepository;
    private readonly ILogger<GetMyReviewHandler> _logger;

    public GetMyReviewHandler(
        IValidator<GetReviewFilterRequest> validator,
        IReviewsRepository reviewsRepository, 
        ILogger<GetMyReviewHandler> logger)
    {
        _validator = validator;
        _reviewsRepository = reviewsRepository;
        _logger = logger;
    }

    public async Task<PagedResult<GetReviewResponse>> Handle(GetMyReviewCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ReviewValidationException(validationResult.ToErrors());
        }

        var filterWithUserId = command.Request with { UserId = command.UserId };
        var result = await _reviewsRepository.GetReviewsAsync(filterWithUserId, cancellationToken);

        var responses = result.Items.Select(review => new GetReviewResponse(
            review.Id,
            review.UserId,
            review.GameName,
            review.Description,
            review.CreatedAt,
            review.ReviewTags.Select(rt => rt.Tag.Name).ToList()
        )).ToList();

        _logger.LogInformation("Retrieved {Count} reviews for user {UserId} on page {Page} with page size {PageSize}",
            responses.Count,
            command.UserId,
            result.Page,
            result.PageSize);
        
        return new PagedResult<GetReviewResponse>(responses, result.TotalCount, result.Page, result.PageSize);
    }
}
