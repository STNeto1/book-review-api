using System.Security.Claims;
using book_review_api.Data;
using book_review_api.Entities;
using book_review_api.Graph.Inputs;
using book_review_api.Graph.Type;
using Microsoft.EntityFrameworkCore;
using Path = System.IO.Path;

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

    private string GetImageRootFolder(int id)
    {
        var basePath = Path.Combine("wwwroot", "images");
        var bookPath = Path.Combine(basePath, id.ToString());


        // check if book folder exists
        if (!Directory.Exists(bookPath))
        {
            Directory.CreateDirectory(bookPath);
        }

        return bookPath;
    }

    public async Task<PublicBookReview> CreateBookReview(CreateBookReviewInput input,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var user = await _authService.Profile(claimsPrincipal, cancellationToken);

        // open transaction
        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
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

            var root = GetImageRootFolder(bookReview.Id);
            var images = new List<PublicBookImage>();

            foreach (var image in input.Images)
            {
                var fileExtension = Path.GetExtension(image.Name);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";

                var filePath = Path.Combine(root, fileName);
                await using var stream = File.Create(filePath);
                await image.CopyToAsync(stream, cancellationToken);

                var bookReviewImage = new BookReviewImage
                {
                    ImageUrl = $"{bookReview.Id}/{fileName}",
                    BookReviewId = bookReview.Id
                };
                await _context.BookReviewImages.AddAsync(bookReviewImage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                images.Add(new PublicBookImage
                {
                    Id = bookReviewImage.Id,
                    ImageUrl = bookReviewImage.ImageUrl
                });
            }

            await transaction.CommitAsync(cancellationToken);

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
                Images = images,
                CreatedAt = bookReview.CreatedAt
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IEnumerable<PublicBookReview>> GetBookReviews(
        SearchBookReviewInput input,
        CancellationToken cancellationToken)
    {
        var query = _context.BookReviews
            .Include(x => x.Author)
            .Include(x => x.Images)
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

        return bookReviews.Select(ParseBook);
    }

    public async Task<IEnumerable<PublicBookReview>> GetUserBookReviews(SearchBookReviewInput input,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var user = await _authService.Profile(claimsPrincipal, cancellationToken);

        var query = _context.BookReviews
            .Include(x => x.Author)
            .Include(x => x.Images)
            .Where(x => x.UserId == user.Id)
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

        var bookReviews = await query
            .ToListAsync(cancellationToken);

        return bookReviews.Select(ParseBook);
    }

    public async Task<PublicBookReview?> GetBookReview(int id, CancellationToken cancellationToken)
    {
        var bookReview = await _context.BookReviews
            .Include(x => x.Author)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (bookReview == null)
        {
            return null;
        }

        return ParseBook(bookReview);
    }
    
    private PublicBookReview ParseBook(BookReview bookReview)
    {
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
            Images = bookReview.Images.Select(y => new PublicBookImage
            {
                Id = y.Id,
                ImageUrl = y.ImageUrl
            }).ToList(),
            CreatedAt = bookReview.CreatedAt
        };
    }
}