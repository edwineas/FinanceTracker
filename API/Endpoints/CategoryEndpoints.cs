using System.Security.Claims;
using FinanceTracker.Infrasturecture.Data;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.API.Endpoints;

public static class CategoryEndpoints
{
  public static IEndpointRouteBuilder MapCategoryEndpoints (this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/categories").RequireAuthorization();

    group.MapPost("/", async (CreateCategoryRequest newCategory, ClaimsPrincipal user, AppDbContext db) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var category = new Category
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        Name = newCategory.Name,
        CreatedAt = DateTime.UtcNow
      };

      db.Categories.Add(category);
      await db.SaveChangesAsync();

      return Results.Created($"/categories/{category.Id}", category);
    });

    group.MapGet("/", async (ClaimsPrincipal user, AppDbContext db) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var categories = await db.Categories.Where(category => category.UserId == userId).ToListAsync();

      return Results.Ok(categories);
    });

    return app;
  }
}
