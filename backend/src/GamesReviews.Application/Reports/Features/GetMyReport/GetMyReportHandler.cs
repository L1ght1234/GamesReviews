using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Reports.Exceptions;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Reports;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Reports.Features.GetMyReport;

public class GetMyReportHandler : ICommandHandler<PagedResult<GetReportResponse>, GetMyReportCommand>
{
    private readonly IReportsRepository _reportsRepository;
    private readonly IValidator<GetReportsFilterRequest> _validator;
    private readonly ILogger<GetMyReportHandler> _logger;

    public GetMyReportHandler(
        IReportsRepository reportsRepository, 
        IValidator<GetReportsFilterRequest> validator, 
        ILogger<GetMyReportHandler> logger)
    {
        _reportsRepository = reportsRepository;
        _validator = validator;
        _logger = logger;
    }
    public async Task<PagedResult<GetReportResponse>> Handle(GetMyReportCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.GetReportsFilterRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ReportValidationException(validationResult.ToErrors());
        }

        var filterWithUserId = command.GetReportsFilterRequest with { UserId = command.UserId };
        var result = await _reportsRepository.GetReportsAsync(filterWithUserId, cancellationToken);

        var responses = result.Items.Select(report => new GetReportResponse(
            report.Id,
            report.UserId,
            report.ReportedUserId,
            report.ContentId,
            report.ContentType,
            report.Reason,
            report.Description,
            report.Status,
            report.CreatedAt,
            report.UpdatedAt,
            report.ModeratorId
        )).ToList();

        _logger.LogInformation("User {UserId} retrieved {Count} reports on page {Page} with page size {PageSize}",
            command.UserId,
            responses.Count,
            result.Page,
            result.PageSize);
        
        return new PagedResult<GetReportResponse>(responses, result.TotalCount, result.Page, result.PageSize);
    }
}