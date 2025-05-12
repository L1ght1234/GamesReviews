using GamesReviews.Domain.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GamesReviews.Infrastructure.Configurations;

public class ReviewTagConfiguration : IEntityTypeConfiguration<ReviewTag>
{
    public void Configure(EntityTypeBuilder<ReviewTag> builder)
    {
        builder.HasKey(rt => new { rt.ReviewId, rt.TagId }); 
        
        builder.HasOne(rt => rt.Review)
            .WithMany(r => r.ReviewTags)
            .HasForeignKey(rt => rt.ReviewId);
            
        builder.HasOne(rt => rt.Tag)
            .WithMany(t => t.ReviewTags)
            .HasForeignKey(rt => rt.TagId);
    }
}