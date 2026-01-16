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

    return app;
  }
}
