using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GamesReviews.Contracts.Users;

public class UpdateUserRequestForModeration
{
    [JsonIgnore]
    [BindNever] 
    public Guid UserId { get; set; }
    public string UserName { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
    public string Email { get; set; } = default!;
}