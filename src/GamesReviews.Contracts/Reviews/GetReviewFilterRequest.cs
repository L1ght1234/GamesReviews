using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GamesReviews.Contracts.Reviews;

public record GetReviewFilterRequest
{
    public string? Search { get; init; } = null;
    public string? Tag { get; init; } = null;
    public string? SortBy { get; init; } = "GameName";
    public string? SortDirection { get; init; } = "asc";
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    [BindNever]
    [JsonIgnore]
    public Guid? UserId { get; init; }
}
