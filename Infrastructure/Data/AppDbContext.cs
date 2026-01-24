using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  public DbSet<User> Users => Set<User>();
  public DbSet<Account> Accounts => Set<Account>();
  public DbSet<Category> Categories => Set<Category>();
  public DbSet<Transaction> Transactions => Set<Transaction>();
  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Account>()
      .HasOne(a => a.User)
      .WithMany()
      .HasForeignKey(a => a.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Category>()
      .HasOne(c => c.User)
      .WithMany()
      .HasForeignKey(c => c.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Transaction>()
      .HasOne(t => t.User)
      .WithMany()
      .HasForeignKey(t => t.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Transaction>()
      .HasOne(t => t.FromAccount)
      .WithMany()
      .HasForeignKey(t => t.FromAccountId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Transaction>()
      .HasOne(t => t.ToAccount)
      .WithMany()
      .HasForeignKey(t => t.ToAccountId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Transaction>()
      .HasOne(t => t.Category)
      .WithMany()
      .HasForeignKey(t => t.CategoryId)
      .OnDelete(DeleteBehavior.SetNull);

    modelBuilder.Entity<RefreshToken>()
      .HasOne(rt => rt.User)
      .WithMany()
      .HasForeignKey(rt => rt.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
