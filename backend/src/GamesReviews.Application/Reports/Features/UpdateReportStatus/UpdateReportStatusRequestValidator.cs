using FluentValidation;
using GamesReviews.Contracts.Reports;

namespace GamesReviews.Application.Reports.Features.UpdateReportStatus;

public class UpdateReportStatusRequestValidator : AbstractValidator<UpdateReportStatusRequest>
{
    public UpdateReportStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid status.");
    }
}