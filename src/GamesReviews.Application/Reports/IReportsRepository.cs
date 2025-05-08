using GamesReviews.Contracts;
using GamesReviews.Contracts.Reports;
using GamesReviews.Domain.Reports;

namespace GamesReviews.Application.Reports;

public interface IReportsRepository
{
    Task<Guid> AddAsync(Report report, CancellationToken cancellationToken);
    Task<PagedResult<Report>> GetReportsAsync(GetReportsFilterRequest filter, CancellationToken cancellationToken);
    Task<Report?> GetByIdAsync(Guid reportId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}