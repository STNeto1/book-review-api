using book_review_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace book_review_api.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
}