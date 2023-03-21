using book_review_api.Data;
using book_review_api.Graph;
using book_review_api.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextPool<DataContext>(options => { options.UseMySQL(connectionString); });

builder
    .Services
    .AddScoped<IAuthService, AuthService>()
    .AddGraphQLServer()
    .AddMutationType<Mutation>()
    .AddQueryType<Query>()
    .AddMutationConventions();

var app = builder.Build();

app.MapGraphQL();

app.Run();