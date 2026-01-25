using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Application.DTOs.Response;
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

  public async Task<(bool Success, object? Error, Transaction? Transaction)> CreateAsync(CreateTransactionRequest request, Guid userId)
  {
    if (request.FromAccount.HasValue)
    {
      var exists = await _accountRepo.ExistsAsync(request.FromAccount.Value, userId);
      if (!exists) return (false, new { FromAccount = "Invalid Source Account" }, null);
    }

    if (request.ToAccount.HasValue)
    {
      var exists = await _accountRepo.ExistsAsync(request.ToAccount.Value, userId);
      if (!exists) return (false, new { ToAccount = "Invalid Destination Account" }, null);
    }

    if (request.Category.HasValue)
    {
      var exists = await _categoryRepo.ExistsAsync(request.Category.Value, userId);
      if (!exists) return (false, new { Category = "Invalid Category" }, null);
    }

    var transaction = new Transaction
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      Type = request.Type,
      Amount = request.Amount!.Value,
      FromAccountId = request.FromAccount,
      ToAccountId = request.ToAccount,
      CategoryId = request.Category,
      Note = request.Note,
      Date = string.IsNullOrEmpty(request.Date) 
        ? DateTime.UtcNow 
        : DateTime.SpecifyKind(DateTime.Parse(request.Date), DateTimeKind.Utc),  // Ensure UTC kind
      CreatedAt = DateTime.UtcNow
    };

    await _repo.AddAsync(transaction);

    return (true, null, transaction);
  }

  public async Task<(bool Success, object? Error, Transaction? Transaction)> UpdateAsync(UpdateTransactionRequest request, Guid userId)
  {
    var existingTransaction = await _repo.GetByIdAsync(request.Id);
    if (existingTransaction == null || existingTransaction.UserId != userId)
    {
      return (false, new { Id = "Transaction not found" }, null);
    }

    if (request.FromAccount.HasValue)
    {
      var exists = await _accountRepo.ExistsAsync(request.FromAccount.Value, userId);
      if (!exists) return (false, new { FromAccount = "Invalid Source Account" }, null);
    }

    if (request.ToAccount.HasValue)
    {
      var exists = await _accountRepo.ExistsAsync(request.ToAccount.Value, userId);
      if (!exists) return (false, new { ToAccount = "Invalid Destination Account" }, null);
    }

    if (request.Category.HasValue)
    {
      var exists = await _categoryRepo.ExistsAsync(request.Category.Value, userId);
      if (!exists) return (false, new { Category = "Invalid Category" }, null);
    }

    existingTransaction.Type = request.Type;
    existingTransaction.Amount = request.Amount!.Value;
    existingTransaction.FromAccountId = request.FromAccount;
    existingTransaction.ToAccountId = request.ToAccount;
    existingTransaction.CategoryId = request.Category;
    existingTransaction.Note = request.Note;
    existingTransaction.Date = string.IsNullOrEmpty(request.Date) 
      ? existingTransaction.Date
      : DateTime.SpecifyKind(DateTime.Parse(request.Date), DateTimeKind.Utc);

    await _repo.UpdateAsync(existingTransaction);
    return (true, null, existingTransaction);
  }

  public async Task<(bool success, object? Error, List<TransactionListingResponse>? transactions)> GetAllAsync(Guid userId, string? type, Guid? accountId, Guid? categoryId, DateTime? startDate, DateTime? endDate)
  {
    if (accountId.HasValue)
    {
      var exists = await _accountRepo.ExistsAsync(accountId.Value, userId);
      if (!exists) return (false, new { AccountId = "Invalid Destination Account" }, null);
    }

    if (categoryId.HasValue)
    {
      var exists = await _categoryRepo.ExistsAsync(categoryId.Value, userId);
      if (!exists) return (false, new { CategoryId = "Invalid Category" }, null);
    }

    var transactions = await _repo.GetAllAsync(userId, type, accountId, categoryId, startDate, endDate);

    return (true, null, transactions.Select(transaction => new TransactionListingResponse
    {
      Id = transaction.Id,
      Date = transaction.Date,
      Type = transaction.Type,
      Category = transaction.Category?.Name ?? string.Empty,
      CategoryId = transaction.Category?.Id ?? null,
      Amount = transaction.Amount,
      FromAccount = transaction.FromAccount?.Name ?? string.Empty,
      FromAccountId = transaction.FromAccount?.Id ?? null,
      ToAccount = transaction.ToAccount?.Name ?? string.Empty,
      ToAccountId = transaction.ToAccount?.Id ?? null,
      Note = transaction.Note ?? string.Empty
    }).ToList());
  }

  public async Task<List<TransactionSummaryResponse>> GetLatestAsync(Guid userId, int limit)
  {
    var transaction = await _repo.GetLatestAsync(userId, limit);
    return transaction.Select(transaction => new TransactionSummaryResponse
    {
      Date = transaction.Date,
      Type = transaction.Type,
      Category = transaction.Category?.Name ?? string.Empty,
      Amount = transaction.Amount,
      Account = transaction.Type switch
      {
        "Income" => transaction.ToAccount?.Name ?? string.Empty,
        "Expense" => transaction.FromAccount?.Name ?? string.Empty,
        "Transfer" => transaction.FromAccount?.Name ?? string.Empty,
        _ => string.Empty
      }
    }).ToList();
  }
}
