using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

}
