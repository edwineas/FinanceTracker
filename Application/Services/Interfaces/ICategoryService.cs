using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services.Interfaces;

public interface ICategoryService
{
  Task<Category> CreateAsync(CreateCategoryRequest request, Guid userId);
  Task<List<Category>> GetAllAsync(Guid userId);
}
