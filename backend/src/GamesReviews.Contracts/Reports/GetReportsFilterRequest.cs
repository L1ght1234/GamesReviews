using System.Text.Json.Serialization;
using GamesReviews.Domain.Reports;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GamesReviews.Contracts.Reports;

public record GetReportsFilterRequest
{
    public string? Search { get; init; } = null;
    public ReportStatus? Status { get; init; } = ReportStatus.InProgress;
    public string? SortBy { get; init; } = "CreatedAt";
    public string? SortDirection { get; init; } = "asc";
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    [BindNever]
    [JsonIgnore]
    public Guid? UserId { get; init; }
}