using GamesReviews.Domain.Reviews;

namespace GamesReviews.Domain.Tags;

public class Tag
{
    private Tag() { }

    private Tag(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    
    public List<ReviewTag> ReviewTags { get; private set; } = [];

    public static (Tag? Tag, List<string> Errors) Create(
        Guid id,
        string name)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("Name is required");

        if (errors.Any())
            return (null, errors);

        return (new Tag(id, name), errors);
    }
}
