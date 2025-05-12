namespace GamesReviews.Contracts.Comments;

public record CommentResponse(
    Guid Id,
    string Text,
    Guid UserId,
    string UserName,
    Guid ReviewId,
    DateTime CreatedAt,
    Guid? ParentCommentId);