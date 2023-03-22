using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using book_review_api.Data;
using book_review_api.Entities;
using book_review_api.Exceptions;
using book_review_api.Graph.Inputs;
using book_review_api.Graph.Output;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace book_review_api.Service;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse> Register(RegisterInput input, CancellationToken cancellationToken)
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
            
            return GenerateToken(user);
        }
        catch (UserAlreadyExistsException e)
        {
            await transaction.RollbackAsync(cancellationToken);

            throw e;
        }
    }

    public async Task<AuthResponse> Login(LoginInput input, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == input.Email, cancellationToken);
            if (user == null)
            {
                throw new InvalidCredentialsException();
            }

            if (!BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
            {
                throw new InvalidCredentialsException();
            }

            await transaction.CommitAsync(cancellationToken);

            return GenerateToken(user);
        }
        catch (InvalidCredentialsException e)
        {
            await transaction.RollbackAsync(cancellationToken);

            throw e;
        }
    }
    
    public async Task<User> Profile(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var user = await GetUserFromClaims(claimsPrincipal, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedException();
        }

        return user;
    }

    private AuthResponse GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);


        var userClaims = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = userClaims,
            Expires = DateTime.Now.AddDays(1),
            Issuer = "https://auth.stneto.dev",
            Audience = "https://graphql.stneto.dev",
            SigningCredentials = cred
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenJwt = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(tokenJwt);

        return new AuthResponse
        {
            Token = token
        };
    }
    
    private ValueTask<User> GetUserFromClaims(ClaimsPrincipal claims, CancellationToken cancellationToken)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var parsed = userId.IsNullOrEmpty() ? -1 : int.Parse(userId);
        
        return _context.Users.FindAsync(parsed, cancellationToken);
    }
}