using GamesReviews.Application.Reports;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Reports;
using GamesReviews.Domain.Reports;
using Microsoft.EntityFrameworkCore;

namespace GamesReviews.Infrastructure.Repositories;

public class ReportsRepository : IReportsRepository
{
    private readonly GamesReviewsDbContext _context;

    public ReportsRepository(GamesReviewsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(Report report, CancellationToken cancellationToken)
    {
        await _context.Reports.AddAsync(report, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return report.Id;
    }
    
    public async Task<Report?> GetByIdAsync(Guid reportId, CancellationToken cancellationToken)
    {
        return await _context.Reports
            .FirstOrDefaultAsync(r => r.Id == reportId, cancellationToken);
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<PagedResult<Report>> GetReportsAsync(GetReportsFilterRequest filter, CancellationToken cancellationToken)
    {
        var query = _context.Reports.AsQueryable();

        if (filter.UserId.HasValue)
        {
            query = query.Where(r => r.UserId == filter.UserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(r =>
                r.Reason.Contains(filter.Search) ||
                r.Description.Contains(filter.Search));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(r => r.Status == filter.Status.Value);
        }

        query = ApplySorting(query, filter.SortBy, filter.SortDirection);

        var totalCount = await query.CountAsync(cancellationToken);

        var reports = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Report>(reports, totalCount, filter.Page, filter.PageSize);
    }

    private IQueryable<Report> ApplySorting(IQueryable<Report> query, string? sortBy, string? sortDirection)
    {
        var isDescending = sortDirection?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "createdat" => isDescending
                ? query.OrderByDescending(r => r.CreatedAt)
                : query.OrderBy(r => r.CreatedAt),
            "status" => isDescending
                ? query.OrderByDescending(r => r.Status)
                : query.OrderBy(r => r.Status),
            _ => query.OrderBy(r => r.CreatedAt) // по умолчанию — старые вверху
        };
    }
}