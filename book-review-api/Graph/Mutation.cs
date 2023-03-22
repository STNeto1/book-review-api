using System.Security.Claims;
using book_review_api.Exceptions;
using book_review_api.Graph.Inputs;
using book_review_api.Graph.Output;
using book_review_api.Graph.Type;
using book_review_api.Service;
using HotChocolate.Authorization;

namespace book_review_api.Graph;

[Authorize]
public class Mutation
{
    [Error(typeof(UserAlreadyExistsException))]
    public async Task<AuthResponse> Register([Service] IAuthService _authService, RegisterInput input,
        CancellationToken cancellationToken)
    {
        return await _authService.Register(input, cancellationToken);
    }

    [Error(typeof(InvalidCredentialsException))]
    public async Task<AuthResponse> Login([Service] IAuthService _authService, LoginInput input,
        CancellationToken cancellationToken)
    {
        return await _authService.Login(input, cancellationToken);
    }

    [Authorize]
    [Error(typeof(UnauthorizedException))]
    public async Task<PublicBookReview> CreateBookReview([Service] IBookReviewService _bookReviewService,
        CreateBookReviewInput input,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        return await _bookReviewService.CreateBookReview(input, claimsPrincipal, cancellationToken);
    }
}