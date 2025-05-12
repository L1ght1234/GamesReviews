using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Users.Features.Login;
using GamesReviews.Application.Users.Features.Register;
using GamesReviews.Contracts.Users;
using Microsoft.AspNetCore.Mvc;

namespace GamesReviews.Presenters.Users;


[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromServices] ICommandHandler<Guid, RegisterCommand> handler,
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(request);
        
        var userId = await handler.Handle(command, cancellationToken);
        
        return Ok(userId);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromServices] ICommandHandler<string, LoginCommand> handler,
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request);
        
        var token = await handler.Handle(command, cancellationToken);
        
        HttpContext.Response.Cookies.Append("tasty-cookies", token);
        
        return Ok(token);
    }
}