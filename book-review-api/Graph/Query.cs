using System.Security.Claims;
using book_review_api.Exceptions;
using book_review_api.Graph.Type;
using book_review_api.Service;
using HotChocolate.Authorization;

namespace book_review_api.Graph;

[Authorize]
public class Query
{
    public DateTime Timestamp() => DateTime.UtcNow;


    [Authorize]
    [Error(typeof(UnauthorizedException))]
    public async Task<Profile> Profile([Service] IAuthService _authService, ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var user = await _authService.Profile(claimsPrincipal, cancellationToken);
        return Type.Profile.FromEntity(user);
    }
}