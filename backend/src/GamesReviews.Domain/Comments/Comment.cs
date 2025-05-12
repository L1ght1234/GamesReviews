using GamesReviews.Domain.Reviews;
using GamesReviews.Domain.Users;

namespace GamesReviews.Domain.Comments;

public class Comment
{
    private Comment() { }
    private Comment(
        Guid id,
        Guid userId,
        Guid reviewId,
        string text,
        Guid? parentCommentId,
        DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        ReviewId = reviewId;
        Text = text;
        ParentCommentId = parentCommentId;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ReviewId { get; private set; }
    public Guid? ParentCommentId { get; private set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    
    public Review? Review { get; private set; }
    public User? User { get; private set; }
    public Comment? ParentComment { get; private set; }
    public List<Comment> Replies { get; private set; } = [];

    public static (Comment? Comment, List<string> Errors) Create(
        Guid id,
        Guid userId,
        Guid reviewId,
        string text,
        Guid? parentCommentId = null)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(text))
            errors.Add("text cannot be empty");

        if (userId == Guid.Empty)
            errors.Add("invalid user id");

        if (reviewId == Guid.Empty)
            errors.Add("invalid review id");

        if (errors.Any())
            return (null, errors);

        return (new Comment(
            id,
            userId,
            reviewId,
            text,
            parentCommentId,
            DateTime.UtcNow), errors);
    }

    public void Update(string text)
    {
        Text = text;
    }
}