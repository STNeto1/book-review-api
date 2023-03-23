namespace book_review_api.Entities;

public class BookReviewImage
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }

    public int BookReviewId { get; set; }
}