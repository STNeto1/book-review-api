using System.Text;
using book_review_api.Data;
using book_review_api.Graph;
using book_review_api.Patch;
using book_review_api.Service;
using DataAnnotatedModelValidations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var token = Encoding.UTF8.GetBytes(
        builder.Configuration.GetSection("AppSettings:Token").Value!);

    var signingKey = new SymmetricSecurityKey(
        token);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "https://auth.stneto.dev",
        ValidAudience = "https://graphql.stneto.dev",
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey
    };
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextPool<DataContext>(options => { options.UseMySQL(connectionString); });

builder
    .Services
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IBookReviewService, BookReviewService>()
    .AddGraphQLServer()
    .AddAuthorization()
    .AddMutationType<Mutation>()
    .AddQueryType<Query>()
    .AddDataAnnotationsValidator()
    .AddMutationConventions()
    .AllowIntrospection(false)
    .AddHttpRequestInterceptor<IntrospectionInspector>();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapGraphQL());


app.Run();