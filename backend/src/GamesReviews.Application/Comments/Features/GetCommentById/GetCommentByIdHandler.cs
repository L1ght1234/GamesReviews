using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Comments.Excaptions;
using GamesReviews.Application.Extensions;
using GamesReviews.Contracts.Comments;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Comments.Features.GetCommentById;

public class GetCommentByIdHandler : ICommandHandler<CommentResponse, GetCommentByIdCommand>
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly ILogger<GetCommentByIdHandler> _logger;

    public GetCommentByIdHandler(
        ICommentsRepository commentsRepository, 
        ILogger<GetCommentByIdHandler> logger)
    {
        _commentsRepository = commentsRepository;
        _logger = logger;
    }

    public async Task<CommentResponse> Handle(GetCommentByIdCommand command, CancellationToken cancellationToken)
    {
        var comment = await _commentsRepository.GetByIdAsync(command.CommentId, cancellationToken);
        
        if (comment == null)
            throw new CommentNotFoundException(command.CommentId.ToNotFoundError("Review not found."));
        
        _logger.LogInformation("Retrieved comment with id: {CommentId}", command.CommentId);

        return new CommentResponse(
            comment.Id,
            comment.Text,
            comment.UserId,
            comment.User!.UserName ?? "Unknown",
            comment.ReviewId,
            comment.CreatedAt,
            comment.ParentCommentId
        );
    }
}