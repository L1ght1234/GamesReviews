using GamesReviews.Domain.Reports;

namespace GamesReviews.Contracts.Reports;

public record GetReportResponse(
    Guid Id,
    Guid UserId,
    Guid ReportedUserId,
    Guid ContentId,
    ReportedContentType ContentType,
    string Reason,
    string Description,
    ReportStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid? ModeratorId);