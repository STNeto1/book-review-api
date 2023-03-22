using System.Security.Claims;
using book_review_api.Data;
using book_review_api.Entities;
using book_review_api.Graph.Inputs;
using book_review_api.Graph.Type;

namespace book_review_api.Service;

public class BookReviewService : IBookReviewService
{
    private readonly DataContext _context;
    private readonly IAuthService _authService;

    public BookReviewService(DataContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<PublicBookReview> CreateBookReview(CreateBookReviewInput input,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var user = await _authService.Profile(claimsPrincipal, cancellationToken);

        var bookReview = new BookReview
        {
            Title = input.Title,
            Content = input.Content,
            Rating = input.Rating,

            BookTitle = input.BookTitle,
            BookAuthor = input.BookAuthor,
            BookYear = input.BookYear,

            UserId = user.Id
        };

        await _context.BookReviews.AddAsync(bookReview, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new PublicBookReview
        {
            Id = bookReview.Id,
            Title = bookReview.Title,
            Content = bookReview.Content,
            Rating = bookReview.Rating,
            BookTitle = bookReview.BookTitle,
            BookAuthor = bookReview.BookAuthor,
            BookYear = bookReview.BookYear,
            Author = new Profile
            {
                Id = user.Id,
                Email = user.Email
            },
            CreatedAt = bookReview.CreatedAt
        };
    }
}