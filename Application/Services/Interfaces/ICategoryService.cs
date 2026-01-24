using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Application.DTOs.Response;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services.Interfaces;

public interface ICategoryService
{
  Task<Category> CreateAsync(CreateCategoryRequest request, Guid userId);
  Task<List<CategoryResponse>> GetAllAsync(Guid userId);
  Task<(bool Success, string? Error)> DeleteAsync(Guid categoryId, Guid userId);
}
