using System.Text.Json.Serialization;

namespace Shared;

public record Error
{
    public string Code { get; }
    public string Message { get; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType Type { get; }
    public string? InvalidField { get; }

    [JsonConstructor]
    private Error(string code, string message, ErrorType type, string? invalidField =  null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }
    
    public static Error NotFound(string? code, string message, Guid? id) 
        => new(code ?? "record.not.found", message, ErrorType.NotFound);
    
    public static Error Validation(string? code, string message, string? invalidField = null) 
        => new(code ?? "value.is.invalid", message, ErrorType.Validation, invalidField);
    
    public static Error Conflict(string? code, string message) 
        => new(code ?? "value.is.conflict", message, ErrorType.Conflict);
    
    public static Error Failure(string? code, string message) 
        => new(code ?? "failure", message, ErrorType.Failure);
    
    public static Error NotFound(string? code, string message) 
        => new(code ?? "record.not.found", message, ErrorType.NotFound);
    
    public static Error Forbidden(string? code, string message) =>
        new(code ?? "forbidden", message, ErrorType.Forbidden);
}

public enum ErrorType
{
    Validation,
    NotFound,
    Failure,
    Conflict,
    Forbidden
}