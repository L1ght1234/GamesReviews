using GamesReviews.Contracts;
using GamesReviews.Contracts.Comments;
using GamesReviews.Domain.Comments;

namespace GamesReviews.Application.Comments;

public interface ICommentsRepository
{
    Task<Comment?> GetByIdAsync(Guid commentId, CancellationToken cancellationToken);
    Task<Guid> AddAsync(Comment comment, CancellationToken cancellationToken);
    Task<Guid> DeleteAsync(Guid commentId, CancellationToken cancellationToken);
    Task<PagedResult<Comment>> GetRootCommentsAsync(Guid reviewId, GetCommentsRequest filter, CancellationToken cancellationToken);
    Task<PagedResult<Comment>> GetRepliesAsync(Guid parentCommentId, GetCommentsRequest filter, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}