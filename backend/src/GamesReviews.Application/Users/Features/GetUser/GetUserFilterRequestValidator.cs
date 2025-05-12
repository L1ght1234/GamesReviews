using FluentValidation;
using GamesReviews.Contracts.Users;

namespace GamesReviews.Application.Users.Features.GetUser;

public class GetUserFilterRequestValidator : AbstractValidator<GetUserFilterRequest>
{
    public GetUserFilterRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.SortDirection)
            .Must(x => string.IsNullOrEmpty(x) || x.ToLower() == "asc" || x.ToLower() == "desc")
            .WithMessage("Sort direction must be 'asc' or 'desc'");

        RuleFor(x => x.SortBy)
            .Must(x => string.IsNullOrEmpty(x) || AllowedSortFields.Contains(x))
            .WithMessage("Sort by field is not valid");
    }
    private static readonly string[] AllowedSortFields =
    {
        "UserName",
        "Email",
        "Role"
    };
}