using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Repositories.Interfaces;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
  private readonly AppDbContext _db;

  public TransactionRepository(AppDbContext db) => _db = db;

  public async Task AddAsync(Transaction transaction)
  {
    _db.Transactions.Add(transaction);
    await _db.SaveChangesAsync();
  }

  public async Task<List<Transaction>> GetAllAsync(Guid userId, string? type, Guid? accountId, Guid? categoryId, DateTime? startDate, DateTime? endDate)
  {
    var query = _db.Transactions.Where(transaction => transaction.UserId == userId).AsQueryable();

    if (!string.IsNullOrWhiteSpace(type)) query = query.Where(transaction => transaction.Type == type);
    if (accountId.HasValue) query = query.Where(transaction => transaction.FromAccountId == accountId || transaction.ToAccountId == accountId);
    if (categoryId.HasValue) query = query.Where(transaction => transaction.FromAccountId == accountId || transaction.ToAccountId == accountId);
    if (startDate.HasValue) query = query.Where(transaction => transaction.Date >= startDate);
    if (endDate.HasValue) query = query.Where(transaction => transaction.Date <= endDate);

    return await query.OrderByDescending(transaction => transaction.Date).ToListAsync();
  }

  public async Task<decimal> GetTotalIncomeAsync(Guid userId, DateTime startDate, DateTime endDate) =>
    await _db.Transactions
      .Where(transaction => transaction.UserId == userId && transaction.Type == "Income" && transaction.Date >= startDate && transaction.Date <= endDate)
        .SumAsync(transaction => transaction.Amount);

  public async Task<decimal> GetTotalExpenseAsync(Guid userId, DateTime startDate, DateTime endDate) =>
    await _db.Transactions
      .Where(transaction => transaction.UserId == userId && transaction.Type == "Expense" && transaction.Date >= startDate && transaction.Date <= endDate)
        .SumAsync(transaction => transaction.Amount);

}
