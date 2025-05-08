using GamesReviews.Contracts;
using GamesReviews.Contracts.Users;
using GamesReviews.Domain.Users;

namespace GamesReviews.Application.Users;

public interface IUsersRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Guid> AddAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task<Guid> DeleteAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<PagedResult<User>> GetUsersAsync(GetUserFilterRequest filter, CancellationToken cancellationToken);
}