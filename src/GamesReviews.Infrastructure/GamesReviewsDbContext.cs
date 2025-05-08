using GamesReviews.Domain.Comments;
using GamesReviews.Domain.Reports;
using GamesReviews.Domain.Reviews;
using GamesReviews.Domain.Tags;
using GamesReviews.Domain.Users;
using GamesReviews.Infrastructure.Configurations;
using GamesReviews.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;

namespace GamesReviews.Infrastructure;

public class GamesReviewsDbContext(DbContextOptions<GamesReviewsDbContext> options) : DbContext(options)
{
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ReviewTag> ReviewTags { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewConfiguration());
        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new ReportConfiguration());
        modelBuilder.ApplyConfiguration(new AdminUserSeeder());
        modelBuilder.ApplyConfiguration(new ReviewTagConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }

}