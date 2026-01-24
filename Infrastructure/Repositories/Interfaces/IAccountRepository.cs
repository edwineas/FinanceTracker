using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Infrastructure.Repositories.Interfaces;

public interface IAccountRepository
{
  Task AddAsync(Account account);
  Task<List<Account>> GetByUserAsync(Guid userId);
  Task<bool> ExistsAsync(Guid accountId, Guid userId);
  Task<decimal> GetBalanceAsync(Guid accountId, Guid userId);
  Task<decimal> GetTotalBalanceAsync(Guid userId);
  Task DeleteAsync(Guid accountId);
}
