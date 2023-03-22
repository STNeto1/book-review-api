using System.Security.Claims;
using book_review_api.Exceptions;
using book_review_api.Graph.Inputs;
using book_review_api.Graph.Type;
using book_review_api.Service;
using HotChocolate.Authorization;

namespace book_review_api.Graph;

[Authorize]
public class Query
{
    [Authorize]
    [Error(typeof(UnauthorizedException))]
    public async Task<Profile> Profile([Service] IAuthService _authService, ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var user = await _authService.Profile(claimsPrincipal, cancellationToken);
        return Type.Profile.FromEntity(user);
    }
    
    [Authorize, UsePaging]
    public async Task<IEnumerable<PublicBookReview>> UserBookReviews([Service] IBookReviewService _bookReviewService,
        SearchBookReviewInput input,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        return await _bookReviewService.GetUserBookReviews(input, claimsPrincipal, cancellationToken);
    }


    [UsePaging]
    public async Task<IEnumerable<PublicBookReview>> BookReviews([Service] IBookReviewService _bookReviewService,
        SearchBookReviewInput input,
        CancellationToken cancellationToken)
    {
        return await _bookReviewService.GetBookReviews(input, cancellationToken);
    }

    public async Task<PublicBookReview?> BookReview([Service] IBookReviewService _bookReviewService,
        int id,
        CancellationToken cancellationToken)
    {
        return await _bookReviewService.GetBookReview(id, cancellationToken);
    }
}