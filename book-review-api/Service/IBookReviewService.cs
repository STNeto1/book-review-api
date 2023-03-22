using System.Security.Claims;
using book_review_api.Graph.Inputs;
using book_review_api.Graph.Type;

namespace book_review_api.Service;

public interface IBookReviewService
{
    public Task<PublicBookReview> CreateBookReview(CreateBookReviewInput input, ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken);

    public Task<IEnumerable<PublicBookReview>> GetBookReviews(SearchBookReviewInput input,
        CancellationToken cancellationToken);
    
    public Task<PublicBookReview> GetBookReview(int id, CancellationToken cancellationToken);
}