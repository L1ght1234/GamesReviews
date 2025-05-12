using GamesReviews.Domain.Reports;

namespace GamesReviews.Contracts.Reports;

public record CreateReportRequest(
    Guid ReportedUserId,
    Guid ContentId,
    ReportedContentType ContentType,
    string Reason,
    string Description);