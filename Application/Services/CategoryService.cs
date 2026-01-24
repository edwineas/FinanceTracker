using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Application.DTOs.Response;
using FinanceTracker.Application.Services.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Repositories.Interfaces;

namespace FinanceTracker.Application.Services;

public class CategoryService : ICategoryService
{
  private readonly ICategoryRepository _repo;

  public CategoryService(ICategoryRepository repo) => _repo = repo;

  public async Task<Category> CreateAsync(CreateCategoryRequest request, Guid userId)
  {
    var category = new Category
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      Name = request.Name,
      CreatedAt = DateTime.UtcNow
    };

    await _repo.AddAsync(category);

    return category;
  }

  public async Task<List<CategoryResponse>> GetAllAsync(Guid userId)
  {
    var categories = await _repo.GetAllByUserAsync(userId);

    return categories.Select(category => new CategoryResponse
    {
      Id = category.Id,
      Name =  category.Name
    }).ToList();
  }

  public async Task<(bool Success, string? Error)> DeleteAsync(Guid categoryId, Guid userId)
  {
    var exists = await _repo.ExistsAsync(categoryId, userId);
    if (!exists) return (false, "Category not found");

    await _repo.DeleteAsync(categoryId);
    return (true, null);
  }
}
