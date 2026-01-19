using System.Security.Claims;
using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Application.Services.Interfaces;

namespace FinanceTracker.API.Endpoints;

public static class CategoryEndpoints
{
  public static IEndpointRouteBuilder MapCategoryEndpoints (this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/categories").RequireAuthorization();

    group.MapPost("/", async (CreateCategoryRequest newCategory, ClaimsPrincipal user, ICategoryService categoryService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
      var category = await categoryService.CreateAsync(newCategory, userId);

      return Results.Created($"/categories/{category.Id}", category);
    });

    group.MapGet("/", async (ClaimsPrincipal user, ICategoryService categoryService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var categories = await categoryService.GetAllAsync(userId);

      return Results.Ok(categories);
    });

    return app;
  }
}
