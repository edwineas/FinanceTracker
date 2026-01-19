using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Application.Services.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Repositories.Interfaces;

namespace FinanceTracker.Application.Services;

public class TransactionService : ITransactionService
{
  private readonly ITransactionRepository _repo;
  private readonly IAccountRepository _accountRepo;
  private readonly ICategoryRepository _categoryRepo;

  public TransactionService(ITransactionRepository repo, IAccountRepository accountRepo, ICategoryRepository categoryRepo)
  {
    _repo = repo;
    _accountRepo = accountRepo;
    _categoryRepo = categoryRepo;
  }

  public async Task<(bool Success, string? Error, Transaction? Transaction)> CreateAsync(CreateTransactionRequest request, Guid userId)
  {
    if (request.FromAccountId.HasValue)
    {
      var exists = await _accountRepo.ExistsAsync(request.FromAccountId.Value, userId);
      if (!exists) return (false, "Invalid Source Account", null);
    }

    if (request.ToAccountId.HasValue)
    {
      var exists = await _accountRepo.ExistsAsync(request.ToAccountId.Value, userId);
      if (!exists) return (false, "Invalid Destination Account", null);
    }

    if (request.CategoryId.HasValue)
    {
      var exists = await _categoryRepo.ExistsAsync(request.CategoryId.Value, userId);
      if (!exists) return (false, "Invalid Category", null);
    }

    var transaction = new Transaction
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      Type = request.Type,
      Amount = request.Amount!.Value,
      FromAccountId = request.FromAccountId,
      ToAccountId = request.ToAccountId,
      CategoryId = request.CategoryId,
      Note = request.Note,
      Date = request.Date ?? DateTime.UtcNow,
      CreatedAt = DateTime.UtcNow
    };

    await _repo.AddAsync(transaction);

    return (true, null, transaction);
  }

  public async Task<(bool success, string? Error, List<Transaction>? transactions)> GetAllAsync(Guid userId, string? type, Guid? accountId, Guid? categoryId, DateTime? startDate, DateTime? endDate)
  {
    if (accountId.HasValue)
    {
      var exists = await _accountRepo.ExistsAsync(accountId.Value, userId);
      if (!exists) return (false, "Invalid Source Account", null);
    }

    if (categoryId.HasValue)
    {
      var exists = await _categoryRepo.ExistsAsync(categoryId.Value, userId);
      if (!exists) return (false, "Invalid Category", null);
    }

    var transactions = await _repo.GetAllAsync(userId, type, accountId, categoryId, startDate, endDate);

    return (true, null, transactions);
  }
}
