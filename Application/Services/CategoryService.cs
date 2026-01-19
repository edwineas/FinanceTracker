using FinanceTracker.Application.DTOs.Request;
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

  public async Task<List<Category>> GetAllAsync(Guid userId) => await _repo.GetAllByUserAsync(userId);
}
