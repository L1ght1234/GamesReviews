using FluentValidation;
using GamesReviews.Contracts.Reviews;

namespace GamesReviews.Application.Reviews.Features.UpdateReview;

public class UpdateReviewValidator : AbstractValidator<UpdateReviewRequest>
{
    public UpdateReviewValidator()
    {
        RuleFor(x => x.GameName)
            .MaximumLength(50).WithMessage("Game name must not exceed 50 characters");
        
        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");
    }
}