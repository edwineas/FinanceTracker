
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Infrastructure.Repositories.Interfaces;

public interface ITransactionRepository
{
  Task AddAsync(Transaction transaction);
  Task<List<Transaction>> GetAllAsync(Guid userId, string? type, Guid? accountId, Guid? categoryId, DateTime? startDate, DateTime? endDate);
  Task<decimal> GetTotalIncomeAsync(Guid userId, DateTime startDate, DateTime endDate);
  Task<decimal> GetTotalExpenseAsync(Guid userId, DateTime startDate, DateTime endDate);
  Task<List<Transaction>> GetByAccountIdsAsync(Guid userId, List<Guid> accountIds);
  Task<List<Transaction>> GetLatestAsync(Guid userId, int limit);
}
