using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Comments.Excaptions;
using GamesReviews.Application.Extensions;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Comments;
using GamesReviews.Domain.Comments;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Comments.Features.GetComments;

public class GetCommentsHandler : ICommandHandler<PagedResult<CommentResponse>, GetCommentsCommand>
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly IValidator<GetCommentsRequest> _validator;
    private readonly ILogger<GetCommentsHandler> _logger;

    public GetCommentsHandler(
        ICommentsRepository commentsRepository,
        IValidator<GetCommentsRequest> validator,
        ILogger<GetCommentsHandler> logger)
    {
        _commentsRepository = commentsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<PagedResult<CommentResponse>> Handle(GetCommentsCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.GetCommentsRequest, cancellationToken);
        if (!validationResult.IsValid)
            throw new CommentValidationException(validationResult.ToErrors());

        PagedResult<Comment> pagedComments;

        if (command.ParentCommentId is null)
        {
            pagedComments = await _commentsRepository.GetRootCommentsAsync(
                command.ReviewId, command.GetCommentsRequest, cancellationToken);
        }
        else
        {
            pagedComments = await _commentsRepository.GetRepliesAsync(
                command.ParentCommentId.Value, command.GetCommentsRequest, cancellationToken);
        }

        var responses = pagedComments.Items
            .Select(c => new CommentResponse(
                c.Id,
                c.Text,
                c.UserId,
                c.User!.UserName ?? "Unknown",
                c.CreatedAt,
                c.ParentCommentId))
            .ToList();

        _logger.LogInformation("Returning {Count} comments (total: {TotalCount}, page: {Page}, pageSize: {PageSize})",
            responses.Count,
            pagedComments.TotalCount,
            pagedComments.Page,
            pagedComments.PageSize);

        return new PagedResult<CommentResponse>(
            responses,
            pagedComments.TotalCount,
            pagedComments.Page,
            pagedComments.PageSize);
    }
}
