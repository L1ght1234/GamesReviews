using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Reviews.Exceptions;
using GamesReviews.Application.Tags;
using GamesReviews.Application.Tags.Exceptions;
using GamesReviews.Contracts.Reviews;
using GamesReviews.Domain.Reviews;
using GamesReviews.Domain.Tags;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Reviews.Features.CreateReview;

public class CreateReviewHandler : ICommandHandler<Guid, CreateReviewCommand>
{
    private readonly IReviewsRepository _reviewsRepository;
    private readonly ITagsRepository _tagsRepository;
    private readonly ILogger<CreateReviewHandler> _logger;
    private readonly IValidator<CreateReviewRequest> _validator;

    public CreateReviewHandler(
        IReviewsRepository reviewsRepository,
        ILogger<CreateReviewHandler> logger,
        IValidator<CreateReviewRequest> validator, 
        ITagsRepository tagsRepository)
    {
        _reviewsRepository = reviewsRepository;
        _logger = logger;
        _validator = validator;
        _tagsRepository = tagsRepository;
    }
    
    public async Task<Guid> Handle(CreateReviewCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.CreateReviewRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ReviewValidationException(validationResult.ToErrors());
        }

        var tags = new List<Tag>();

        foreach (var tagName in command.CreateReviewRequest.Tags.Distinct())
        {
            var existingTag = await _tagsRepository.GetByNameAsync(tagName, cancellationToken);
            if (existingTag is not null)
            {
                tags.Add(existingTag);
            }
            else
            {
                var (newTag, errors) = Tag.Create(Guid.NewGuid(), name: tagName);
                if (errors.Any())
                {
                    throw new TagValidationException(errors.ToValidationErrors());
                }
                await _tagsRepository.AddAsync(newTag!, cancellationToken);
                tags.Add(newTag!);
            }
        }

        var (review, reviewErrors) = Review.Create(
            id: Guid.NewGuid(),
            userId: command.UserId,
            gameName: command.CreateReviewRequest.GameName,
            description: command.CreateReviewRequest.Description);

        if (reviewErrors.Any())
        {
            throw new ReviewValidationException(reviewErrors.ToValidationErrors());
        }

        foreach (var tag in tags)
        {
            var reviewTag = new ReviewTag(review!.Id, tag.Id);
            review!.ReviewTags.Add(reviewTag);
        }

        await _reviewsRepository.AddAsync(review!, cancellationToken);

        _logger.LogInformation("Created review with id {ReviewId} by user {UserId}", review!.Id, command.UserId);

        return review.Id; 
    }
}