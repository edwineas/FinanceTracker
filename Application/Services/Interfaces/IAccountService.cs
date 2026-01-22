using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Application.DTOs.Response;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services.Interfaces;

public interface IAccountService
{
  Task<Account> CreateAsync(CreateAccountRequest request, Guid userId);
  Task<List<Account>> GetAllAsync(Guid userId);
  Task<decimal?> GetBalanceAsync(Guid accountId, Guid userId);
  Task<List<AccountWithBalanceResponse>> GetAllWithBalancesAsync(Guid userId);
}
