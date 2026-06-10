using Microsoft.EntityFrameworkCore;
using portfolio_api.Models;

namespace portfolio_api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Contact> Contacts { get; set; }
}
