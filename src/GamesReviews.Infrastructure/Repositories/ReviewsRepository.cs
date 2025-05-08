using GamesReviews.Application.Reviews;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Reviews;
using GamesReviews.Domain.Reviews;
using Microsoft.EntityFrameworkCore;

namespace GamesReviews.Infrastructure.Repositories;

public class ReviewsRepository : IReviewsRepository
{
    private readonly GamesReviewsDbContext _context;

    public ReviewsRepository(GamesReviewsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(Review review, CancellationToken cancellationToken)
    {
        await _context.Reviews.AddAsync(review, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return review.Id;
    }

    public async Task<PagedResult<Review>> GetReviewsAsync(GetReviewFilterRequest filter, CancellationToken cancellationToken)
    {
        var query = _context.Reviews
            .Include(r => r.ReviewTags)
            .ThenInclude(rt => rt.Tag) // подтягиваем теги
            .AsQueryable();

        // Поиск по названию игры
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(r => r.GameName.Contains(filter.Search));
        }

        if (filter.UserId.HasValue)
        {
            query = query.Where(r => r.UserId == filter.UserId.Value);
        }
        
        // Фильтрация по тегу
        if (!string.IsNullOrWhiteSpace(filter.Tag))
        {
            query = query.Where(r => r.ReviewTags.Any(rt => rt.Tag.Name == filter.Tag));
        }

        // Сортировка
        query = ApplySorting(query, filter.SortBy, filter.SortDirection);

        // Получаем общее количество элементов до пагинации
        var totalCount = await query.CountAsync(cancellationToken);

        // Пагинация
        query = query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        var reviews = await query.ToListAsync(cancellationToken);

        return new PagedResult<Review>(reviews, totalCount, filter.Page, filter.PageSize);
    }

    public async Task<Review?> GetByIdAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .Include(r => r.ReviewTags)
            .ThenInclude(rt => rt.Tag)
            .FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);
    }

    public async Task<Guid> DeleteAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        bool reviewExists = await _context.Reviews
            .AnyAsync(u => u.Id == reviewId, cancellationToken);

        if (!reviewExists)
        {
            throw new Exception("Review not found");
        }

        await _context.Reviews
            .Where(w => w.Id == reviewId)
            .ExecuteDeleteAsync(cancellationToken);

        return reviewId;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    private IQueryable<Review> ApplySorting(IQueryable<Review> query, string? sortBy, string? sortDirection)
    {
        var isDescending = sortDirection?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "createdat" => isDescending
                ? query.OrderByDescending(r => r.CreatedAt)
                : query.OrderBy(r => r.CreatedAt),

            "gamename" => isDescending
                ? query.OrderByDescending(r => r.GameName)
                : query.OrderBy(r => r.GameName),

            _ => query.OrderBy(r => r.GameName) // сортировка по GameName по умолчанию
        };
    }
}