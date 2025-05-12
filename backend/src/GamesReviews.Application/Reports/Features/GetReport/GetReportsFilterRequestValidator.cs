using FluentValidation;
using GamesReviews.Contracts.Reports;

namespace GamesReviews.Application.Reports.Features.GetReport;

public class GetReportsFilterRequestValidator : AbstractValidator<GetReportsFilterRequest>
{
    public GetReportsFilterRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");
        
        RuleFor(x => x.SortDirection)
            .Must(sd => sd == null || sd.ToLower() is "asc" or "desc")
            .WithMessage("SortDirection must be either 'asc' or 'desc'");
    }
}