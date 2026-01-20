using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services.Interfaces;

public interface ITransactionService
{
  Task<(bool Success, object? Error, Transaction? Transaction)> CreateAsync(CreateTransactionRequest request, Guid userId);
  Task<(bool success, object? Error, List<Transaction>? transactions)> GetAllAsync(Guid userId, string? type, Guid? accountId, Guid? categoryId, DateTime? startDate, DateTime? endDate);
}
