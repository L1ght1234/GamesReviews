using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Reports;

namespace GamesReviews.Application.Reports.Features.UpdateReportStatus;

public record UpdateReportStatusCommand(
    Guid ReportId,
    Guid ModeratorId,
    UpdateReportStatusRequest UpdateReportStatusRequest) : ICommand;