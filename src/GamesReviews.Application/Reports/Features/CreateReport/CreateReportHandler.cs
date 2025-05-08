using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Comments;
using GamesReviews.Application.Comments.Excaptions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Reports.Exceptions;
using GamesReviews.Application.Reviews;
using GamesReviews.Application.Reviews.Exceptions;
using GamesReviews.Contracts.Reports;
using GamesReviews.Domain.Reports;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Reports.Features.CreateReport;

public class CreateReportHandler : ICommandHandler<Guid, CreateReportCommand>
{
    private readonly IValidator<CreateReportRequest> _validator;
    private readonly IReportsRepository _reportsRepository;
    private readonly IReviewsRepository _reviewRepository;
    private readonly ICommentsRepository _commentRepository;
    private readonly ILogger<CreateReportHandler> _logger;

    public CreateReportHandler(
        IValidator<CreateReportRequest> validator, 
        IReportsRepository reportsRepository, 
        IReviewsRepository reviewRepository, 
        ICommentsRepository commentRepository, 
        ILogger<CreateReportHandler> logger)
    {
        _validator = validator;
        _reportsRepository = reportsRepository;
        _reviewRepository = reviewRepository;
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateReportCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.CreateReportRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ReportValidationException(validationResult.ToErrors());
        }

        Guid reportedUserId = command.CreateReportRequest.ContentType switch
        {
            ReportedContentType.Review => await GetReviewAuthorIdOrThrow(command.CreateReportRequest.ContentId, cancellationToken),
            ReportedContentType.Comment => await GetCommentAuthorIdOrThrow(command.CreateReportRequest.ContentId, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(command.CreateReportRequest.ContentType), "Unsupported content type")
        };

        var (report, creationErrors) = Report.Create(
            id: Guid.NewGuid(),
            userId: command.CurrentUserId,
            reportedUserId: reportedUserId,
            contentId: command.CreateReportRequest.ContentId,
            contentType: command.CreateReportRequest.ContentType,
            reason: command.CreateReportRequest.Reason,
            description: command.CreateReportRequest.Description
        );

        if (creationErrors.Any())
        {
            throw new ReportValidationException(creationErrors.ToValidationErrors());
        }

        await _reportsRepository.AddAsync(report!, cancellationToken);

        _logger.LogInformation("Created report with id {ReportId} by user {UserId} on content {ContentId} ({ContentType})",
            report!.Id, command.CurrentUserId, command.CreateReportRequest.ContentId, command.CreateReportRequest.ContentType);

        return report.Id;
    }

    private async Task<Guid> GetReviewAuthorIdOrThrow(Guid reviewId, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
        if (review is null)
        {
            throw new ReviewNotFoundException(reviewId.ToNotFoundError("Review"));
        }

        return review.UserId;
    }

    private async Task<Guid> GetCommentAuthorIdOrThrow(Guid commentId, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId, cancellationToken);
        if (comment is null)
        {
            throw new CommentNotFoundException(commentId.ToNotFoundError("Comment"));
        }

        return comment.UserId;
    }
}