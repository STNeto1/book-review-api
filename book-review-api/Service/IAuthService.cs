using System.Security.Claims;
using book_review_api.Entities;
using book_review_api.Graph.Inputs;
using book_review_api.Graph.Output;

namespace book_review_api.Service;

public interface IAuthService
{
    public Task<AuthResponse> Register(RegisterInput input, CancellationToken cancellationToken);
    public Task<AuthResponse> Login(LoginInput input, CancellationToken cancellationToken);
    public Task<User> Profile(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken);
}