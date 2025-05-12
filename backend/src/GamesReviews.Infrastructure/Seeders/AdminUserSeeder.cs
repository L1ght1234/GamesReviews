using GamesReviews.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GamesReviews.Infrastructure.Seeders;

public class AdminUserSeeder : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var hashedPassword = "$2a$11$EmyLvPJ/8B1rAJxphPEcA..boxC3dGYKzFvjedpL.Eq3hEiVT8Vo.";

        builder.HasData(new
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserName = "admin",
            HashedPassword = hashedPassword,
            Email = "admin@example.com",
            Role = "Admin"
        });
    }
}