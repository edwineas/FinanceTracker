using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Repositories.Interfaces;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
  private readonly AppDbContext _db;

  public AccountRepository(AppDbContext db) => _db = db;

  public async Task AddAsync(Account account)
  {
    _db.Accounts.Add(account);
    await _db.SaveChangesAsync();
  }

  public async Task<List<Account>> GetByUserAsync(Guid userId) => await _db.Accounts.Where(account => account.UserId == userId).ToListAsync();

  public async Task<bool> ExistsAsync(Guid accountId, Guid userId) => await _db.Accounts.AnyAsync(account => account.Id == accountId && account.UserId == userId);

  public async Task<decimal> GetBalanceAsync(Guid accountId, Guid userId) =>
    await _db.Transactions
      .Where(transaction => transaction.UserId == userId)
        .SumAsync(transaction =>
          (transaction.Type == "Income" && transaction.ToAccountId == accountId ? transaction.Amount : 0) +
          (transaction.Type == "Transfer" && transaction.ToAccountId == accountId ? transaction.Amount : 0) -
          (transaction.Type == "Expense" && transaction.FromAccountId == accountId ? transaction.Amount : 0) -
          (transaction.Type == "Transfer" && transaction.FromAccountId == accountId ? transaction.Amount : 0)
        );
  
  public async Task<decimal> GetTotalBalanceAsync(Guid userId) => 
    await _db.Transactions
      .Where(transaction => transaction.UserId == userId)
        .SumAsync(transaction => transaction.Type == "Income" ? transaction.Amount : transaction.Type == "Expense" ? -transaction.Amount : 0);

  public async Task DeleteAsync(Guid accountId)
  {
    var account = await _db.Accounts.FindAsync(accountId);
    if (account != null)
    {
      _db.Accounts.Remove(account);
      await _db.SaveChangesAsync();
    }
  }
}
