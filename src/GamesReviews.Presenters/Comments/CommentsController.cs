using GamesReviews.Application.Abstractions;
using GamesReviews.Application.AuthMethods;
using GamesReviews.Application.Comments.Features.CreateComment;
using GamesReviews.Application.Comments.Features.DeleteComment;
using GamesReviews.Application.Comments.Features.GetComments;
using GamesReviews.Application.Comments.Features.UpdateComment;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesReviews.Presenters.Comments;

[ApiController]
[Route("reviews/{reviewId:guid}/comments")]
public class CommentsController: ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateRootComment(
        [FromRoute] Guid reviewId,
        [FromBody] CreateCommentRequest request,
        [FromServices] ICommandHandler<Guid, CreateCommentCommand> handler,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        
        var command = new CreateCommentCommand(userId, reviewId, request);
        
        var commentId = await handler.Handle(command, cancellationToken);
        
        return Ok(commentId);
    }
    
    [Authorize]
    [HttpPost("{parentCommentId:guid}/replies")]
    public async Task<IActionResult> CreateReply(
        [FromRoute] Guid reviewId,
        [FromRoute] Guid parentCommentId,
        [FromBody] CreateCommentRequest request,
        [FromServices] ICommandHandler<Guid, CreateCommentCommand> handler,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        
        var command = new CreateCommentCommand(userId, reviewId, request, parentCommentId);
        
        var commentId = await handler.Handle(command, cancellationToken);
        
        return Ok(commentId);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRootComments(
        [FromRoute] Guid reviewId,
        [FromQuery] GetCommentsRequest request,
        [FromServices] ICommandHandler<PagedResult<CommentResponse>, GetCommentsCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetCommentsCommand(reviewId, request);
        
        var comments = await handler.Handle(command, cancellationToken);
        
        return Ok(comments);
    }
    
    [HttpGet("{parentCommentId:guid}/replies")]
    public async Task<IActionResult> GetCommentReplies(
        [FromRoute] Guid reviewId,
        [FromRoute] Guid parentCommentId,
        [FromQuery] GetCommentsRequest request,
        [FromServices] ICommandHandler<PagedResult<CommentResponse>, GetCommentsCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetCommentsCommand(reviewId, request, parentCommentId);
        
        var comments = await handler.Handle(command, cancellationToken);
        
        return Ok(comments);
    }

    [HttpDelete("{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(
        [FromRoute] Guid commentId,
        [FromRoute] Guid reviewId,
        [FromServices] ICommandHandler<Guid, DeleteCommentCommand> handler,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var userRole = User.GetRole();

        var command = new DeleteCommentCommand(reviewId, commentId, userId, userRole);
        
        var deletedId = await handler.Handle(command, cancellationToken);
        
        return Ok(deletedId);
    }
    
    [HttpPut("{commentId:guid}")]
    public async Task<IActionResult> UpdateComment(
        [FromRoute] Guid commentId,
        [FromRoute] Guid reviewId,
        [FromBody] UpdateCommentRequest request,
        [FromServices] ICommandHandler<Guid, UpdateCommentCommand> handler,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var userRole = User.GetRole();

        var command = new UpdateCommentCommand(reviewId, commentId, userId, userRole, request);
        
        var updatedId = await handler.Handle(command, cancellationToken);
        
        return Ok(updatedId);
    }
}