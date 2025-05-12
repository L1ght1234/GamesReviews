using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Reports;

namespace GamesReviews.Application.Reports.Features.GetMyReport;

public record GetMyReportCommand(Guid UserId, GetReportsFilterRequest GetReportsFilterRequest) : ICommand;