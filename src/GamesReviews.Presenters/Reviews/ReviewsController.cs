using GamesReviews.Application.Abstractions;
using GamesReviews.Application.AuthMethods;
using GamesReviews.Application.Reviews.Features.CreateReview;
using GamesReviews.Application.Reviews.Features.DeleteReview;
using GamesReviews.Application.Reviews.Features.GetMyReview;
using GamesReviews.Application.Reviews.Features.GetReview;
using GamesReviews.Application.Reviews.Features.GetReviewById;
using GamesReviews.Application.Reviews.Features.UpdateReview;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesReviews.Presenters.Reviews;

[ApiController]
[Route("reviews")]
public class ReviewsController : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview(
        [FromServices] ICommandHandler<Guid, CreateReviewCommand> handler,
        [FromBody] CreateReviewRequest reviewRequest,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        
        var command = new CreateReviewCommand(userId, reviewRequest);
        
        var reviewId = await handler.Handle(command, cancellationToken);
        
        return Ok(reviewId);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<GetReviewResponse>>> GetReviews(
        [FromQuery] GetReviewFilterRequest request,
        [FromServices] ICommandHandler<PagedResult<GetReviewResponse>, GetReviewCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetReviewCommand(request);
        
        var reviews = await handler.Handle(command, cancellationToken);
        
        return Ok(reviews);
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<PagedResult<GetReviewResponse>>> GetMyReviews(
        [FromQuery] GetReviewFilterRequest request,
        [FromServices] ICommandHandler<PagedResult<GetReviewResponse>, GetMyReviewCommand> handler,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        
        var command = new GetMyReviewCommand(request, userId);
    
        var reviews = await handler.Handle(command, cancellationToken);
    
        return Ok(reviews);
    }

    [Authorize]
    [HttpPut("{reviewId:guid}")]
    public async Task<IActionResult> UpdateReview(
        [FromRoute] Guid reviewId,
        [FromBody] UpdateReviewRequest request,
        [FromServices] ICommandHandler<Guid, UpdateReviewCommand> handler,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var userRole = User.GetRole();

        var command = new UpdateReviewCommand(reviewId, userId, userRole, request);

        await handler.Handle(command, cancellationToken);

        return Ok(reviewId);
    }

    [Authorize]
    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> DeleteReview(
        [FromServices] ICommandHandler<Guid, DeleteReviewCommand> handler,
        [FromRoute] Guid reviewId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var userRole = User.GetRole();
        
        var command = new DeleteReviewCommand(reviewId, userId, userRole);
        
        await handler.Handle(command, cancellationToken);
        
        return Ok(reviewId);
    }

    [HttpGet("{reviewId:guid}")]
    public async Task<ActionResult<GetReviewResponse>> GetReviewById(
        [FromRoute] Guid reviewId,
        [FromServices] ICommandHandler<GetReviewResponse, GetReviewByIdCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetReviewByIdCommand(reviewId);
        
        var review = await handler.Handle(command, cancellationToken);
        
        return Ok(review);
    }
    
}