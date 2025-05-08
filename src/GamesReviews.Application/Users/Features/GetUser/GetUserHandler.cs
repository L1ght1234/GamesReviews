using FluentValidation;
using GamesReviews.Application.Abstractions;
using GamesReviews.Application.Extensions;
using GamesReviews.Application.Users.Exceptions;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Users;
using Microsoft.Extensions.Logging;

namespace GamesReviews.Application.Users.Features.GetUser;

public class GetUserHandler : ICommandHandler<PagedResult<GetUserResponse>, GetUserCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IValidator<GetUserFilterRequest> _validator;
    private readonly ILogger<GetUserHandler> _logger;

    public GetUserHandler(
        IUsersRepository usersRepository, 
        IValidator<GetUserFilterRequest> validator,
        ILogger<GetUserHandler> logger)
    {
        _usersRepository = usersRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<PagedResult<GetUserResponse>> Handle(GetUserCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.GetUserFilterRequest, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new UserValidationException(validationResult.ToErrors());
        }

        var usersPagedResult = await _usersRepository.GetUsersAsync(command.GetUserFilterRequest, cancellationToken);

        var userResponses = usersPagedResult.Items.Select(user => new GetUserResponse(
            user.Id,
            user.UserName,
            user.Email,
            user.Role
        )).ToList();
        
        _logger.LogInformation("Retrieved {Count} users on page {Page} with page size {PageSize}", 
            userResponses.Count, 
            usersPagedResult.Page, 
            usersPagedResult.PageSize);

        return new PagedResult<GetUserResponse>(
            userResponses,
            usersPagedResult.TotalCount,
            usersPagedResult.Page,
            usersPagedResult.PageSize
        );
    }
}