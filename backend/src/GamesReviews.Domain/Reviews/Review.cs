using GamesReviews.Domain.Comments;
using GamesReviews.Domain.Users;

namespace GamesReviews.Domain.Reviews;

public class Review
{
    private Review() { }

    private Review(
        Guid id,
        Guid userId,
        string gameName,
        string description,
        DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        GameName = gameName;
        Description = description;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string GameName { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public User? User { get; private set; }
    public List<Comment> Comments { get; private set; } = [];
    public List<ReviewTag> ReviewTags  { get; private set; } = [];

    public static (Review? Review, List<string> Errors) Create(
        Guid id,
        Guid userId,
        string gameName,
        string description)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(gameName))
            errors.Add("Game name is required");

        if (string.IsNullOrWhiteSpace(description))
            errors.Add("Description is required");

        if (userId == Guid.Empty)
            errors.Add("Invalid user id");

        if (errors.Any())
            return (null, errors);

        return (new Review(id, userId, gameName, description, DateTime.UtcNow), errors);
    }
    
    public void Update(string newGameName, string newDescription, IEnumerable<Guid> newTagIds)
    {
        GameName = newGameName;
        Description = newDescription;

        ReviewTags.Clear(); 

        foreach (var tagId in newTagIds.Distinct())
        {
            ReviewTags.Add(new ReviewTag(Id, tagId)); 
        }
    }
}

