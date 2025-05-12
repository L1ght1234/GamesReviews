using GamesReviews.Domain.Users;

namespace GamesReviews.Application.AuthMethods;

public interface IJwtProvider
{
    string CreateToken(User user);
}