namespace book_review_api.Entities;

public class BookReview
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }

    public string BookTitle { get; set; }
    public string BookAuthor { get; set; }
    public int BookYear { get; set; }

    public DateTime CreatedAt { get; private set; }
    public int UserId { get; set; }
    public User Author { get; set; }
}