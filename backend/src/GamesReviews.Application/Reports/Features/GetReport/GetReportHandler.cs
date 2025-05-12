using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Reports.Exceptions;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Reports;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Reports.Features.GetReport;

public class GetReportHandler : ICommandHandler<PagedResult<GetReportResponse>, GetReportCommand>
{
    private readonly IReportsRepository _reportsRepository;
    private readonly IValidator<GetReportsFilterRequest> _validator;
    private readonly ILogger<GetReportHandler> _logger;


    public GetReportHandler(
        IReportsRepository reportsRepository, 
        IValidator<GetReportsFilterRequest> validator, 
        ILogger<GetReportHandler> logger)
    {
        _reportsRepository = reportsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<PagedResult<GetReportResponse>> Handle(GetReportCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.GetReportsFilterRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ReportValidationException(validationResult.ToErrors());
        }

        var pagedReports = await _reportsRepository.GetReportsAsync(command.GetReportsFilterRequest, cancellationToken);

        var reportResponses = pagedReports.Items.Select(r => new GetReportResponse(
            r.Id,
            r.UserId,
            r.ReportedUserId,
            r.ContentId,
            r.ContentType,
            r.Reason,
            r.Description,
            r.Status,
            r.CreatedAt,
            r.UpdatedAt,
            r.ModeratorId
        )).ToList();
        
        _logger.LogInformation(
            "Retrieved {Count} reports (Status: {Status}, Page: {Page}, PageSize: {PageSize})",
            reportResponses.Count,
            command.GetReportsFilterRequest.Status?.ToString() ?? "Any",
            pagedReports.Page,
            pagedReports.PageSize);
        
        return new PagedResult<GetReportResponse>(
            reportResponses,
            pagedReports.TotalCount,
            pagedReports.Page,
            pagedReports.PageSize
        );
    }
}