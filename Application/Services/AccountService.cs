using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Application.Services.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Repositories.Interfaces;

namespace FinanceTracker.Application.Services;

public class AccountService : IAccountService
{
  private readonly IAccountRepository _repo;

  public AccountService(IAccountRepository repo) => _repo = repo;

  public async Task<Account> CreateAsync(CreateAccountRequest request, Guid userId)
  {
    var account = new Account
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      Name = request.Name,
      Type = request.Type,
      CreatedAt = DateTime.UtcNow
    };

    await _repo.AddAsync(account);
    return account;
  }

  public async Task<List<Account>> GetAllAsync(Guid userId) => await _repo.GetByUserAsync(userId);

  public async Task<decimal?> GetBalanceAsync(Guid accountId, Guid userId)
  {
    var exists = await _repo.ExistsAsync(accountId, userId);
    if (!exists) return null;

    return await _repo.GetBalanceAsync(accountId, userId);
  }
}
