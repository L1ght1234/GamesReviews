using GamesReviews.Domain.Users;

namespace GamesReviews.Domain.Reports;

public class Report
{
    private Report() { }

    private Report(
        Guid id,
        Guid userId,
        Guid reportedUserId,
        Guid contentId,
        ReportedContentType contentType,
        string reason,
        string description,
        ReportStatus status,
        DateTime createdAt,
        DateTime updatedAt,
        Guid? moderatorId)
    {
        Id = id;
        UserId = userId;
        ReportedUserId = reportedUserId;
        ContentId = contentId;
        ContentType = contentType;
        Reason = reason;
        Description = description;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        ModeratorId = moderatorId;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ReportedUserId { get; private set; }
    public Guid ContentId { get; private set; }
    public ReportedContentType ContentType { get; private set; }
    public string Reason { get; private set; }
    public string Description { get; private set; }
    public ReportStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid? ModeratorId { get; private set; }

    public User? User { get; private set; }
    public User? ReportedUser { get; private set; }
    public User? Moderator { get; private set; }

    public static (Report? Report, List<string> Errors) Create(
        Guid id,
        Guid userId,
        Guid reportedUserId,
        Guid contentId,
        ReportedContentType contentType,
        string reason,
        string description)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(reason))
            errors.Add("Reason is required");

        if (userId == Guid.Empty || reportedUserId == Guid.Empty)
            errors.Add("Invalid user IDs");

        if (contentId == Guid.Empty)
            errors.Add("Invalid content ID");

        if (!Enum.IsDefined(typeof(ReportedContentType), contentType))
            errors.Add("Invalid content type");

        if (errors.Any())
            return (null, errors);

        var report = new Report(
            id,
            userId,
            reportedUserId,
            contentId,
            contentType,
            reason,
            description,
            ReportStatus.InProgress,
            DateTime.UtcNow,
            DateTime.UtcNow,
            null);

        return (report, errors);
    }
    
    public void UpdateStatus(ReportStatus newStatus, Guid moderatorId)
    {
        Status = newStatus;
        ModeratorId = moderatorId;
    }
}
