using System.Security.Claims;
using FinanceTracker.Data;
using FinanceTracker.Data.Models;
using FinanceTracker.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Endpoints;

public static class AccountEndpoints
{
  public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/accounts").RequireAuthorization();

    _ = group.MapPost("/", async (CreateAccountRequest newAccount, ClaimsPrincipal user, AppDbContext db) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var account = new Account
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        Name = newAccount.Name,
        Type = newAccount.Type,
        CreatedAt = DateTime.UtcNow
      };

      db.Accounts.Add(account);
      await db.SaveChangesAsync();

      return Results.Created($"/accounts/{account.Id}", new { Name = account.Name, Type = account.Type });
    });

    group.MapGet("/", async (ClaimsPrincipal user, AppDbContext db) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var accounts = await db.Accounts.Where(account => account.UserId == userId).ToListAsync();

      return Results.Ok(accounts);
    });

    group.MapGet("/{id}/balance", async (Guid id, ClaimsPrincipal user, AppDbContext db) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var exists = await db.Accounts.AnyAsync(account => account.Id == id && account.UserId == userId);

      if (!exists)
      {
        return Results.NotFound();
      }

      var balance = await db.Transactions.Where(transaction => transaction.UserId == userId)
        .SumAsync(transaction =>
          (transaction.Type == "Income" && transaction.ToAccountId == id ? transaction.Amount : 0) +
          (transaction.Type == "Transfer" && transaction.ToAccountId == id ? transaction.Amount : 0) -
          (transaction.Type == "Expense" && transaction.FromAccountId == id ? transaction.Amount : 0) -
          (transaction.Type == "Transfer" && transaction.FromAccountId == id ? transaction.Amount : 0)
        );

      return Results.Ok( new { balance });
    });

    return app;
  }
}
