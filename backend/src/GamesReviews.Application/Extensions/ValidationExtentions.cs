using FluentValidation.Results;
using Shared;

namespace GamesReviews.Application.Extensions;

public static class ValidationExtensions
{
    public static Error[] ToErrors(this ValidationResult result) =>
        result.Errors.Select(e => Error.Validation(e.ErrorCode, e.ErrorMessage, e.PropertyName)).ToArray();
    
    public static Error[] ToValidationErrors(this List<string> errors, string? fieldName = null) =>
        errors.Select(e => Error.Validation(null, e, fieldName)).ToArray();
    
    public static Error[] ToNotFoundError(this Guid id, string entityName) =>
        new[] { Error.NotFound($"{entityName.ToLower()}.not.found", $"{entityName} with id {id} not found", id) };
    
    public static Error[] ToNotFoundError(this string email, string entityName) =>
        new[] { Error.NotFound($"{entityName.ToLower()}.not.found", $"{entityName} with email '{email}' not found") };
    
    public static Error[] ToForbiddenErrors(this string role, string entityName) =>
        new[] { Error.Forbidden($"{entityName.ToLower()}.delete.{role.ToLower()}.forbidden",
                $"You do not have permission to delete a {role} {entityName.ToLower()}.") };
    
    public static Error[] ToForbiddenErrors(this string entityName) =>
        new[] { Error.Forbidden($"{entityName.ToLower()}.access.forbidden",
                $"You do not have permission to modify this {entityName.ToLower()}.") };
    
    public static Error[] ToMismatchErrors(this string entity)
        => new[] { Error.Failure($"{entity.ToLower()}.mismatch", $"{entity} does not belong to the specified parent.") };

}