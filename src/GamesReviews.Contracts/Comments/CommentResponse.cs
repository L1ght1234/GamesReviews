namespace GamesReviews.Contracts.Comments;

public record CommentResponse(
    Guid Id,
    string Text,
    Guid UserId,
    DateTime CreatedAt,
    Guid? ParentCommentId);