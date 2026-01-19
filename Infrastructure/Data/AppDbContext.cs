using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrasturecture.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  public DbSet<User> Users => Set<User>();
  public DbSet<Account> Accounts => Set<Account>();
  public DbSet<Category> Categories => Set<Category>();
  public DbSet<Transaction> Transactions => Set<Transaction>();
}
