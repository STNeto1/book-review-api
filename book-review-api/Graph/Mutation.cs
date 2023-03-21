using book_review_api.Data;
using book_review_api.Entities;
using book_review_api.Exceptions;
using book_review_api.Graph.Inputs;
using Microsoft.EntityFrameworkCore;

namespace book_review_api.Graph;

public class Mutation
{
    [Error(typeof(UserAlreadyExistsException))]
    public async Task<bool> Register(DataContext context, RegisterInput input, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == input.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new UserAlreadyExistsException();
            }

            var user = new User
            {
                Email = input.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password),
            };
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

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