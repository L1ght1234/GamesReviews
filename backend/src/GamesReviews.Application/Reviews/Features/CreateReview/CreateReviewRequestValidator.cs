using FluentValidation;
using GamesReviews.Contracts.Reviews;

namespace GamesReviews.Application.Reviews.Features.CreateReview;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.GameName)
            .MaximumLength(30).WithMessage("Game name cannot exceed 30 characters.");
        
        RuleForEach(x => x.Tags)
            .MaximumLength(20).WithMessage("Tag name cannot exceed 20 characters.");
    }
}