using System.Security.Claims;
using book_review_api.Data;
using book_review_api.Entities;
using book_review_api.Exceptions;
using book_review_api.Graph.Inputs;
using book_review_api.Graph.Type;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<PublicBookReview>> GetBookReviews(
        SearchBookReviewInput input,
        CancellationToken cancellationToken)
    {
        var query = _context.BookReviews
            .Include(x => x.Author)
            .AsQueryable();

        if (!string.IsNullOrEmpty(input.Term))
        {
            query = query.Where(x =>
                x.BookTitle.Contains(input.Term) ||
                x.BookAuthor.Contains(input.Term));
        }

        if (input.Rating != null)
        {
            query = query.Where(x => x.Rating == input.Rating);
        }

        if (input.AuthorId != null)
        {
            query = query.Where(x => x.UserId == input.AuthorId);
        }


        var bookReviews = await query
            .ToListAsync(cancellationToken);

        return bookReviews.Select(x => new PublicBookReview
        {
            Id = x.Id,
            Title = x.Title,
            Content = x.Content,
            Rating = x.Rating,
            BookTitle = x.BookTitle,
            BookAuthor = x.BookAuthor,
            BookYear = x.BookYear,
            Author = new Profile
            {
                Id = x.Author.Id,
                Email = x.Author.Email
            },
            CreatedAt = x.CreatedAt
        });
    }
    
    public async Task<PublicBookReview?> GetBookReview(int id, CancellationToken cancellationToken)
    {
        var bookReview = await _context.BookReviews
            .Include(x => x.Author)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (bookReview == null)
        {
            return null;
        }
        
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
                Id = bookReview.Author.Id,
                Email = bookReview.Author.Email
            },
            CreatedAt = bookReview.CreatedAt
        };
    }
}