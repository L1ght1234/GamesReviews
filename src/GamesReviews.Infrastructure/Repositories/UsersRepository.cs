using GamesReviews.Application.Users;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Users;
using GamesReviews.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace GamesReviews.Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly GamesReviewsDbContext _context;

    public UsersRepository(GamesReviewsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user.Id;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
    
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
    
    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        bool userExists = await _context.Users
            .AnyAsync(u => u.Id == userId, cancellationToken);

        if (!userExists)
        {
            throw new Exception("User not found");
        }

        await _context.Users
            .Where(w => w.Id == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return userId;
    }
    
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken) 
        => await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken) 
        => await _context.Users.AnyAsync(u => u.UserName == username, cancellationToken);

    public async Task<PagedResult<User>> GetUsersAsync(GetUserFilterRequest filter, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsQueryable();

        // Поиск по имени или email
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(u => 
                u.UserName.Contains(filter.Search) || 
                u.Email.Contains(filter.Search));
        }

        // Фильтрация по роли
        if (!string.IsNullOrWhiteSpace(filter.Role))
        {
            query = query.Where(u => u.Role == filter.Role);
        }

        // Сортировка
        query = ApplySorting(query, filter.SortBy, filter.SortDirection);

        // Получаем общее количество элементов до пагинации
        var totalCount = await query.CountAsync();

        // Пагинация
        query = query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        var users = await query.ToListAsync();

        return new PagedResult<User>(users, totalCount, filter.Page, filter.PageSize);
    }
    
    private IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, string? sortDirection)
    {
        bool ascending = sortDirection?.ToLower() != "desc";

        return sortBy?.ToLower() switch
        {
            "email" => ascending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
            "role" => ascending ? query.OrderBy(u => u.Role) : query.OrderByDescending(u => u.Role),
            _ => ascending ? query.OrderBy(u => u.UserName) : query.OrderByDescending(u => u.UserName)
        };
    }
}
