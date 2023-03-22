using System.ComponentModel.DataAnnotations;

namespace book_review_api.Graph.Inputs;

public class CreateBookReviewInput
{
    [Required, MinLength(10)] public string Title { get; set; }
    [Required, MinLength(10)] public string Content { get; set; }

    [Required, Range(0, 5, ErrorMessage = "Value must be between 0 and 5")]
    public int Rating { get; set; }

    [Required, MinLength(10)] public string BookTitle { get; set; }
    [Required, MinLength(10)] public string BookAuthor { get; set; }

    [Required, Range(0, 2023, ErrorMessage = "Value must be between 0 and 2023")]
    public int BookYear { get; set; }
}