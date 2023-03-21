using book_review_api.Exceptions;
using book_review_api.Graph.Inputs;
using book_review_api.Service;

namespace book_review_api.Graph;

public class Mutation
{
    [Error(typeof(UserAlreadyExistsException))]
    public async Task<bool> Register([Service] IAuthService _authService, RegisterInput input,
        CancellationToken cancellationToken)
    {
        return await _authService.Register(input, cancellationToken);
    }
    
    [Error(typeof(InvalidCredentialsException))]
    public async Task<bool> Login([Service] IAuthService _authService, LoginInput input,
        CancellationToken cancellationToken)
    {
        return await _authService.Login(input, cancellationToken);
    }
}