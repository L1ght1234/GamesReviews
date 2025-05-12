using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Exceptions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Reviews.Exceptions;
using GamesReviews.Application.Tags;
using GamesReviews.Application.Tags.Exceptions;
using GamesReviews.Contracts.Reviews;
using GamesReviews.Domain.Tags;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Reviews.Features.UpdateReview;

public class UpdateReviewHandler : ICommandHandler<Guid, UpdateReviewCommand>
{
    private readonly IReviewsRepository _reviewsRepository;
    private readonly ITagsRepository _tagsRepository;
    private readonly IValidator<UpdateReviewRequest> _validator;
    private readonly ILogger<UpdateReviewHandler> _logger;

    public UpdateReviewHandler(
        IReviewsRepository reviewsRepository, 
        ITagsRepository tagsRepository, 
        IValidator<UpdateReviewRequest> validator, 
        ILogger<UpdateReviewHandler> logger)
    {
        _reviewsRepository = reviewsRepository;
        _tagsRepository = tagsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Guid> Handle(UpdateReviewCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.UpdateReviewRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ReviewValidationException(validationResult.ToErrors());
        }

        var existingReview = await _reviewsRepository.GetByIdAsync(command.ReviewId, cancellationToken);
        if (existingReview == null)
            throw new ReviewNotFoundException(command.ReviewId.ToNotFoundError("Review not found."));

        var isOwner = existingReview.UserId == command.UserId;
        var isModerator = command.UserRole == "Admin" || command.UserRole == "Moderator";
        
        if (!isOwner && !isModerator)
        {
            throw new ForbiddenActionException("Review".ToForbiddenErrors());
        }

        var tags = new List<Tag>();
        foreach (var tagName in command.UpdateReviewRequest.Tags.Distinct())
        {
            var tag = await _tagsRepository.GetByNameAsync(tagName, cancellationToken);
            if (tag != null)
            {
                tags.Add(tag);
            }
            else
            {
                var (newTag, errors) = Tag.Create(Guid.NewGuid(), tagName);
                if (errors.Any())
                    throw new TagValidationException(errors.ToValidationErrors());

                await _tagsRepository.AddAsync(newTag!, cancellationToken);
                tags.Add(newTag!);
            }
        }

        existingReview.Update(
            newGameName: command.UpdateReviewRequest.GameName,
            newDescription: command.UpdateReviewRequest.Description,
            newTagIds: tags.Select(t => t.Id)
        );

        await _reviewsRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated review with id: {ReviewId} by user {UserId}", command.ReviewId, command.UserId);
        return command.ReviewId;
    }
}