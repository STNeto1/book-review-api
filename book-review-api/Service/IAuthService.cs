using book_review_api.Graph.Inputs;

namespace book_review_api.Service;

public interface IAuthService
{
    public Task<bool> Register(RegisterInput input, CancellationToken cancellationToken);
}