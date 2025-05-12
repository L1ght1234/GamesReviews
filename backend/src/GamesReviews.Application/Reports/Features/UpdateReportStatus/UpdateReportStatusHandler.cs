using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Reports.Exceptions;
using GamesReviews.Contracts.Reports;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Reports.Features.UpdateReportStatus;

public class UpdateReportStatusHandler : ICommandHandler<Guid, UpdateReportStatusCommand>
{
    private readonly IReportsRepository _reportsRepository;
    private readonly IValidator<UpdateReportStatusRequest> _validator;
    private readonly ILogger<UpdateReportStatusHandler> _logger;

    public UpdateReportStatusHandler(
        IReportsRepository reportsRepository, 
        IValidator<UpdateReportStatusRequest> validator, 
        ILogger<UpdateReportStatusHandler> logger)
    {
        _reportsRepository = reportsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Guid> Handle(UpdateReportStatusCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.UpdateReportStatusRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ReportValidationException(validationResult.ToErrors());
        }

        var report = await _reportsRepository.GetByIdAsync(command.ReportId, cancellationToken);
        if (report is null)
        {
            throw new ReportNotFoundException(command.ReportId.ToNotFoundError("Report not found."));
        }

        report.UpdateStatus(command.UpdateReportStatusRequest.Status, command.ModeratorId);

        await _reportsRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated report {ReportId} to status {Status} by moderator {ModeratorId}",
            report.Id, report.Status, report.ModeratorId);

        return report.Id;
    }
}