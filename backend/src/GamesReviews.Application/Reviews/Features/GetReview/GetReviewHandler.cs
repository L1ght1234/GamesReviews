using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Reviews.Exceptions;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Reviews;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Reviews.Features.GetReview;

public class GetReviewHandler : ICommandHandler<PagedResult<GetReviewResponse>, GetReviewCommand>
{
    private readonly IReviewsRepository _reviewsRepository;
    private readonly IValidator<GetReviewFilterRequest> _validator;
    private readonly ILogger<GetReviewHandler> _logger;

    public GetReviewHandler(
        IReviewsRepository reviewsRepository, 
        IValidator<GetReviewFilterRequest> validator, 
        ILogger<GetReviewHandler> logger)
    {
        _reviewsRepository = reviewsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<PagedResult<GetReviewResponse>> Handle(GetReviewCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.GetReviewFilterRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ReviewValidationException(validationResult.ToErrors());
        }
        
        var reviewsPagedResult = await _reviewsRepository.GetReviewsAsync(command.GetReviewFilterRequest, cancellationToken);

        var reviewResponses = reviewsPagedResult.Items.Select(review => new GetReviewResponse(
            review.Id,
            review.UserId,
            review.GameName,
            review.Description,
            review.CreatedAt,
            review.ReviewTags.Select(rt => rt.Tag.Name).ToList()
        )).ToList();
        
        _logger.LogInformation("Retrieved {Count} reviews on page {Page} with page size {PageSize}",
            reviewResponses.Count,
            reviewsPagedResult.Page,
            reviewsPagedResult.PageSize);

        return new PagedResult<GetReviewResponse>(
            reviewResponses,
            reviewsPagedResult.TotalCount,
            reviewsPagedResult.Page,
            reviewsPagedResult.PageSize
        );
    }
}