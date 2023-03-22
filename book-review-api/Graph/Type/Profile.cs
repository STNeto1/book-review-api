using book_review_api.Entities;

namespace book_review_api.Graph.Type;

public class Profile
{
    public int Id { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; private set; }

    public static Profile FromEntity(User user)
    {
        return new()
        {
            Id = user.Id,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
        };
    }
}