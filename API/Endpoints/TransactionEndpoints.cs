using System.Security.Claims;
using FinanceTracker.API.Responses;
using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Application.Services.Interfaces;

namespace FinanceTracker.API.Endpoints;

public static class TransactionEndpoints
{
  public static IEndpointRouteBuilder MapTransactionEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/transactions").RequireAuthorization();

    group.MapPost("/", async (CreateTransactionRequest newTransaction, ClaimsPrincipal user, ITransactionService transactionService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var (success, error, transaction) = await transactionService.CreateAsync(newTransaction, userId);

      if (!success) return ApiResults.BadRequest("Transaction Creation Failed", error);

      return ApiResults.Created(transaction);
    });

    group.MapGet("/", async (
      string? type,
      Guid? accountId,
      Guid? categoryId,
      DateTime? startDate,
      DateTime? endDate,
      ClaimsPrincipal user,
      ITransactionService transactionService
      ) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var (success, error, transactions) = await transactionService.GetAllAsync(userId, type, accountId, categoryId, startDate, endDate);

      if (!success) return ApiResults.BadRequest("Transaction Listing Failed", error);

      return ApiResults.Ok(transactions);

    });

    group.MapGet("/latest", async (int? limit, ClaimsPrincipal user, ITransactionService transactionService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
      var transactions = await transactionService.GetLatestAsync(userId, limit ?? 5);

      return ApiResults.Ok(transactions);
    });

    group.MapPut("/", async (UpdateTransactionRequest updatedTransaction, ClaimsPrincipal user, ITransactionService transactionService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var (success, error, transaction) = await transactionService.UpdateAsync(updatedTransaction, userId);

      if (!success) return ApiResults.BadRequest("Transaction Update Failed", error);

      return ApiResults.Ok(transaction);
    }); 

    group.MapDelete("/{id}", async (Guid id, ClaimsPrincipal user, ITransactionService transactionService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var (success, error) = await transactionService.DeleteAsync(id, userId);

      if (!success) ApiResults.Conflict("Transaction Deletion Failed", error);

      return ApiResults.Deleted();
    });

    return app;
  }
}
