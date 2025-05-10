namespace GamesReviews.Contracts.Comments;

public record CommentResponse(
    Guid Id,
    string Text,
    Guid UserId,
    string UserName,
    DateTime CreatedAt,
    Guid? ParentCommentId);