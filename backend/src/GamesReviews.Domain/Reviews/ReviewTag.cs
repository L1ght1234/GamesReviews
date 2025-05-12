using GamesReviews.Domain.Tags;

namespace GamesReviews.Domain.Reviews;

public class ReviewTag
{
    public Guid ReviewId { get; private set; }
    public Review Review { get; private set; } = null!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = null!;

    private ReviewTag() { }

    public ReviewTag(Guid reviewId, Guid tagId)
    {
        ReviewId = reviewId;
        TagId = tagId;
    }
}