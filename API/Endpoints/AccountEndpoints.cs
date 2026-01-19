using System.Security.Claims;
using FinanceTracker.Infrasturecture.Data;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Application.DTOs.Request;
using Microsoft.EntityFrameworkCore;
using FinanceTracker.Application.Services.Interfaces;

namespace FinanceTracker.API.Endpoints;

public static class AccountEndpoints
{
  public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/accounts").RequireAuthorization();

    group.MapPost("/", async (CreateAccountRequest newAccount, ClaimsPrincipal user, IAccountService accountService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
      var account = await accountService.CreateAsync(newAccount, userId);

      return Results.Created($"/accounts/{account.Id}", new { Name = account.Name, Type = account.Type });
    });

    group.MapGet("/", async (ClaimsPrincipal user, IAccountService accountService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
      var accounts = await accountService.GetAllAsync(userId);

      return Results.Ok(accounts);
    });

    group.MapGet("/{id}/balance", async (Guid id, ClaimsPrincipal user, IAccountService accountService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
      var balance = await accountService.GetBalanceAsync(id, userId);

      return Results.Ok(new { balance });
    });

    return app;
  }
}
