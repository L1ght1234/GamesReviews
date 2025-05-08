using GamesReviews.Domain.Tags;

namespace GamesReviews.Application.Tags;

public interface ITagsRepository
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Tag>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);
    Task AddAsync(Tag tag, CancellationToken cancellationToken = default);
}