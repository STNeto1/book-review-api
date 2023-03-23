namespace book_review_api.Graph.Type;

public class PublicBookReview
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
    public string BookTitle { get; set; }
    public string BookAuthor { get; set; }
    public int BookYear { get; set; }
    public Profile Author { get; set; }
    public List<PublicBookImage> Images { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PublicBookImage
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
}