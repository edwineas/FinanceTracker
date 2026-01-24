using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Application.DTOs.Response;
using FinanceTracker.Application.Services.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Repositories.Interfaces;

namespace FinanceTracker.Application.Services;

public class AccountService : IAccountService
{
  private readonly IAccountRepository _repo;
  private readonly ITransactionRepository _transactionRepo;

  public AccountService(IAccountRepository repo, ITransactionRepository transactionRepo)
  {
    _repo = repo;
    _transactionRepo = transactionRepo;
  }

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

  public async Task<List<AccountWithBalanceResponse>> GetAllWithBalancesAsync(Guid userId)
  {
    var accounts = await _repo.GetByUserAsync(userId);
    var accountIds = accounts.Select(account => account.Id).ToList();
    var transactions = await _transactionRepo.GetByAccountIdsAsync(userId, accountIds);

    var balances = accounts.ToDictionary(account => account.Id, account => 0m);

    foreach (var transaction in transactions)
    {
      if (transaction.Type == "Income" && transaction.ToAccountId.HasValue && balances.ContainsKey(transaction.ToAccountId.Value))
      {
        balances[transaction.ToAccountId.Value] += transaction.Amount;
      }
      else if (transaction.Type == "Expense" && transaction.FromAccountId.HasValue && balances.ContainsKey(transaction.FromAccountId.Value))
      {
        balances[transaction.FromAccountId.Value] -= transaction.Amount;
      }
      else if (transaction.Type == "Transfer")
      {
        if (transaction.FromAccountId.HasValue && balances.ContainsKey(transaction.FromAccountId.Value))
        {
          balances[transaction.FromAccountId.Value] -= transaction.Amount;
        }
        if (transaction.ToAccountId.HasValue && balances.ContainsKey(transaction.ToAccountId.Value))
        {
          balances[transaction.ToAccountId.Value] += transaction.Amount;
        }
      }
    }

    return accounts.Select(account => new AccountWithBalanceResponse
    {
      Id = account.Id,
      Name = account.Name,
      Type = account.Type,
      Balance = balances[account.Id],
      CreatedAt = account.CreatedAt
    }).ToList();
  }

  public async Task<(bool Success, string? Error)> DeleteAsync(Guid accountId, Guid userId)
  {
    var exists = await _repo.ExistsAsync(accountId, userId);
    if (!exists) return (false, "Account not found");

    await _repo.DeleteAsync(accountId);
    return (true, null);
  }
}
