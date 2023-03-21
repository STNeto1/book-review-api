using book_review_api.Data;
using book_review_api.Graph;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DataContext>(options => options.UseMySQL(connectionString));

builder
    .Services
    .AddGraphQLServer()
    .AddMutationType<Mutation>()
    .AddQueryType<Query>()
    .AddMutationConventions()
    .RegisterDbContext<DataContext>();

var app = builder.Build();

app.MapGraphQL();

app.Run();