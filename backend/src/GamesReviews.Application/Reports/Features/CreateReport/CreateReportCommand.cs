using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Reports;

namespace GamesReviews.Application.Reports.Features.CreateReport;

public record CreateReportCommand(Guid CurrentUserId, CreateReportRequest CreateReportRequest) : ICommand;