using System.Security.Claims;
using FinanceTracker.Data;
using FinanceTracker.Data.Models;
using FinanceTracker.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Endpoints;

public static class TransactionEndpoints
{
  public static IEndpointRouteBuilder MapTransactionEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/transactions").RequireAuthorization();

    group.MapPost("/", async (CreateTransactionRequest newTransaction, ClaimsPrincipal user, AppDbContext db) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      if (newTransaction.FromAccountId.HasValue)
      {
        var exists = await db.Accounts.AnyAsync(account => account.Id == newTransaction.FromAccountId && account.UserId == userId);
        if (!exists)
        {
          return Results.BadRequest("Invalid Source Account");
        }
      }

      if (newTransaction.ToAccountId.HasValue)
      {
        var exists = await db.Accounts.AnyAsync(account => account.Id == newTransaction.ToAccountId && account.UserId == userId);
        if (!exists)
        {
          return Results.BadRequest("Invalid Destination Account");
        }
      }

      if (newTransaction.CategoryId.HasValue)
      {
        var exists = await db.Categories.AnyAsync(category => category.Id == newTransaction.CategoryId && category.UserId == userId);
        if (!exists)
        {
          return Results.BadRequest("Invalid category");
        }
      }

      var transaction = new Transaction
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        Type = newTransaction.Type,
        Amount = newTransaction.Amount!.Value,
        FromAccountId = newTransaction.FromAccountId,
        ToAccountId = newTransaction.ToAccountId,
        CategoryId = newTransaction.CategoryId,
        Note = newTransaction.Note,
        Date = newTransaction.Date ?? DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow
      };

      db.Transactions.Add(transaction);
      await db.SaveChangesAsync();

      return Results.Created($"/transactions/{transaction.Id}", transaction);
    });

    group.MapGet("/", async (
      string? type,
      Guid? accountId, 
      DateTime? startDate,
      DateTime? endDate,
      ClaimsPrincipal user, 
      AppDbContext db
      ) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var query = db.Transactions.Where(transaction => transaction.UserId == userId).AsQueryable();

      if (!string.IsNullOrWhiteSpace(type))
      {
        query = query.Where(transaction => transaction.Type == type);
      }

      if (accountId.HasValue)
      {
        query = query.Where(transaction => transaction.FromAccountId == accountId || transaction.ToAccountId == accountId);
      }

      if (startDate.HasValue)
      {
        query = query.Where(transaction => transaction.Date >= startDate);
      }

      if (endDate.HasValue)
      {
        query = query.Where(transaction => transaction.Date <= endDate);
      }

      var transactions = await query.OrderByDescending(transaction => transaction.Date).ToListAsync();

      return Results.Ok(transactions);

    });

    return app;
  }
}
