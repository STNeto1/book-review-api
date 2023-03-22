namespace book_review_api.Graph.Inputs;

public class SearchBookReviewInput
{
    public string? Term { get; set; }
    public int? Rating { get; set; }
    public int? AuthorId { get; set; }
}