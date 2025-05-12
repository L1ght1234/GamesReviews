using GamesReviews.Application.Comments;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Comments;
using GamesReviews.Domain.Comments;
using Microsoft.EntityFrameworkCore;

namespace GamesReviews.Infrastructure.Repositories;

public class CommentsRepository : ICommentsRepository
{
    private readonly GamesReviewsDbContext _context;

    public CommentsRepository(GamesReviewsDbContext context)
    {
        _context = context;
    }

    public async Task<Comment?> GetByIdAsync(Guid commentId, CancellationToken cancellationToken)
    {
        return await _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);
    }

    public async Task<Guid> AddAsync(Comment comment, CancellationToken cancellationToken)
    {
        await _context.Comments.AddAsync(comment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return comment.Id;
    }

    public async Task<Guid> DeleteAsync(Guid commentId, CancellationToken cancellationToken)
    {
        bool commentExists = await _context.Comments
            .AnyAsync(u => u.Id == commentId, cancellationToken);

        if (!commentExists)
        {
            throw new Exception("comment not found");
        }

        await _context.Comments
            .Where(w => w.Id == commentId)
            .ExecuteDeleteAsync(cancellationToken);

        return commentId;
    }

    public async Task<PagedResult<Comment>> GetRootCommentsAsync(Guid reviewId, GetCommentsRequest filter, CancellationToken cancellationToken)
    {
        var query = _context.Comments
            .Include(c => c.User)
            .Where(c => c.ReviewId == reviewId && c.ParentCommentId == null)
            .OrderByDescending(c => c.CreatedAt)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var comments = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Comment>(comments, totalCount, filter.Page, filter.PageSize);
    }
    
    public async Task<PagedResult<Comment>> GetRepliesAsync(Guid parentCommentId, GetCommentsRequest filter, CancellationToken cancellationToken)
    {
        var query = _context.Comments
            .Include(c => c.User)
            .Where(c => c.ParentCommentId == parentCommentId)
            .OrderBy(c => c.CreatedAt)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var replies = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Comment>(replies, totalCount, filter.Page, filter.PageSize);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}