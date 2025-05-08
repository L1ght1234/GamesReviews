using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Reports;

namespace GamesReviews.Application.Reports.Features.GetReport;

public record GetReportCommand(GetReportsFilterRequest GetReportsFilterRequest) : ICommand;