using System.Security.Claims;
using FinanceTracker.API.Responses;
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

      return ApiResults.Created(category);
    });

    group.MapGet("/", async (ClaimsPrincipal user, ICategoryService categoryService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var categories = await categoryService.GetAllAsync(userId);

      return ApiResults.Ok(categories);
    });

    group.MapDelete("/{id}", async (Guid id, ClaimsPrincipal user, ICategoryService categoryService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
      var (success, error) = await categoryService.DeleteAsync(id, userId);
      if (!success) return ApiResults.Conflict(error!);
      return ApiResults.Deleted();
    });

    return app;
  }
}
