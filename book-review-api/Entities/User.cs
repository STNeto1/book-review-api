namespace book_review_api.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTime.Now;
}