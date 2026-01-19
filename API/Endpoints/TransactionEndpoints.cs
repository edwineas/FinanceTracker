using System.Security.Claims;
using FinanceTracker.Infrasturecture.Data;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Application.DTOs.Request;
using Microsoft.EntityFrameworkCore;
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

      if (!success) return Results.BadRequest(error);

      return Results.Created($"/transactions/{transaction!.Id}", transaction);
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

      return Results.Ok(transactions);

    });

    return app;
  }
}
