using FluentValidation;
using GamesReviews.Contracts.Reviews;

namespace GamesReviews.Application.Reviews.Features.GetReview;

public class GetReviewFilterRequestValidator : AbstractValidator<GetReviewFilterRequest>
{
    public GetReviewFilterRequestValidator()
    {
        RuleFor(x => x.SortBy)
            .Must(value => value is "GameName" or "CreatedAt")
            .WithMessage("SortBy must be either 'GameName' or 'CreatedAt'.");

        RuleFor(x => x.SortDirection)
            .Must(value => value is "asc" or "desc")
            .WithMessage("SortDirection must be either 'asc' or 'desc'.");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");
    }
}