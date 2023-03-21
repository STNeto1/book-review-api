using book_review_api.Data;
using book_review_api.Entities;
using book_review_api.Exceptions;
using book_review_api.Graph.Inputs;
using Microsoft.EntityFrameworkCore;

namespace book_review_api.Service;

public class AuthService : IAuthService
{
    private readonly DataContext _context;

    public AuthService(DataContext context)
    {
        _context = context;
    }

    public async Task<bool> Register(RegisterInput input, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == input.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new UserAlreadyExistsException();
            }

            var user = new User
            {
                Email = input.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password),
            };
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (UserAlreadyExistsException e)
        {
            await transaction.RollbackAsync(cancellationToken);

            throw e;
        }
    }
}