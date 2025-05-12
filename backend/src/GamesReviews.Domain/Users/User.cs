using GamesReviews.Domain.Comments;
using GamesReviews.Domain.Reports;
using GamesReviews.Domain.Reviews;

namespace GamesReviews.Domain.Users;

public class User
{
    private User() {}
    private User(
        Guid id,
        string userName,
        string hashedPassword,
        string email, 
        string role = "User")
    {
        Id = id;
        UserName = userName;
        HashedPassword = hashedPassword;
        Email = email;
        Role = role;
    }

    public Guid Id { get; private set; }
    public string UserName { get; private set; }
    public string HashedPassword { get; private set; }
    public string Email { get; private set; }
    public string Role { get; private set; }
    
    
    public List<Review> Reviews { get; private set; } = [];
    public List<Comment> Comments { get; private set; } = [];
    public List<Report> ReportsCreated { get; private set; } = [];
    public List<Report> ReportsReceived { get; private set; } = [];

    public static (User? User, List<string> Errors) Create(
        Guid id,
        string userName,
        string hashedPassword,
        string email,
        string role = "User")
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(userName))
            errors.Add("user name is required");

        if (string.IsNullOrWhiteSpace(hashedPassword))
            errors.Add("password is required");

        if (string.IsNullOrWhiteSpace(email))
            errors.Add("email address is required");
        
        if(string.IsNullOrWhiteSpace(role))
            errors.Add("role is required");

        if (errors.Any())
            return (null, errors);

        return (new User(
            id,
            userName,
            hashedPassword,
            email,
            role), errors);
    }
    
    public void Update(string newUserName, string newHashedPassword, string newEmail)
    {
        UserName = newUserName;
        HashedPassword = newHashedPassword;
        Email = newEmail;
    }
}