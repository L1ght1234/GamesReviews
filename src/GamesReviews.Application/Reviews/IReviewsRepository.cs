using GamesReviews.Contracts;
using GamesReviews.Contracts.Reviews;
using GamesReviews.Domain.Reviews;

namespace GamesReviews.Application.Reviews;

public interface IReviewsRepository
{
    Task<Guid> AddAsync(Review review, CancellationToken cancellationToken);
    Task<PagedResult<Review>> GetReviewsAsync(GetReviewFilterRequest filter, CancellationToken cancellationToken);
    Task<Review?> GetByIdAsync(Guid reviewId, CancellationToken cancellationToken);
    Task<Guid> DeleteAsync(Guid reviewId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}