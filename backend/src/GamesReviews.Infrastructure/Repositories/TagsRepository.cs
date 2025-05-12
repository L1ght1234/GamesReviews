using GamesReviews.Application.Tags;
using GamesReviews.Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace GamesReviews.Infrastructure.Repositories;

public class TagsRepository : ITagsRepository
{
    private readonly GamesReviewsDbContext _context;

    public TagsRepository(GamesReviewsDbContext context)
    {
        _context = context;
    }

    public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<List<Tag>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .Where(t => names.Contains(t.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        await _context.Tags.AddAsync(tag, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}