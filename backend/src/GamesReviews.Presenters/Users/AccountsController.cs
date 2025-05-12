using GamesReviews.Application.Abstractions;
using GamesReviews.Application.AuthMethods;
using GamesReviews.Application.Users.Features.DeleteUser;
using GamesReviews.Application.Users.Features.GetMyUser;
using GamesReviews.Application.Users.Features.GetUser;
using GamesReviews.Application.Users.Features.RegisterModerator;
using GamesReviews.Application.Users.Features.UpdateUser;
using GamesReviews.Application.Users.Features.UpdateUserForModeration;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesReviews.Presenters.Users;

[ApiController]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateUser(
        [FromServices] ICommandHandler<Guid, UpdateUserCommand> handler,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        request.UserId = userId;
        
        var command = new UpdateUserCommand(userId, request);
        
        await handler.Handle(command, cancellationToken);
        
        return Ok(userId);
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<GetUserResponse>> GetMyUser(
        [FromServices] ICommandHandler<GetUserResponse, GetMyUserCommand> handler,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        
        var command = new GetMyUserCommand(userId);
        
        var user = await handler.Handle(command, cancellationToken);
        
        return Ok(user);
    }
    
    [Authorize(Roles = "Moderator,Admin")]
    [HttpGet("moderation")]
    public async Task<ActionResult<PagedResult<GetUserResponse>>> GetUsers(
        [FromQuery] GetUserFilterRequest request,
        [FromServices] ICommandHandler<PagedResult<GetUserResponse>, GetUserCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetUserCommand(request);
        
        var users = await handler.Handle(command, cancellationToken);
        
        return Ok(users);
    }
    
    [Authorize(Roles = "Moderator,Admin")]
    [HttpPut("moderation/{userId:guid}")]
    public async Task<IActionResult> UpdateUserForModeration(
        [FromRoute] Guid userId,
        [FromServices] ICommandHandler<Guid, UpdateUserForModerationCommand> handler,
        [FromBody] UpdateUserRequestForModeration request,
        CancellationToken cancellationToken)
    {
        request.UserId = userId;
        
        var command = new UpdateUserForModerationCommand(userId, request);
        
        await handler.Handle(command, cancellationToken);
        
        return Ok(userId);
    }
    
    [Authorize(Roles = "Moderator,Admin")]
    [HttpDelete("moderation/{userId:guid}")]
    public async Task<IActionResult> DeleteUserForModeration(
        [FromServices] ICommandHandler<DeleteUserCommand> handler,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(userId);
        
        await handler.Handle(command, cancellationToken);
        
        return Ok(userId);
    }
    
    [Authorize(Roles = "Admin")] 
    [HttpPost("administration/moderators")]
    public async Task<IActionResult> CreateModerator(
        [FromServices] ICommandHandler<RegisterModeratorCommand> handler,
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterModeratorCommand(request);
        
        await handler.Handle(command, cancellationToken);
        
        return Ok();
    }
}