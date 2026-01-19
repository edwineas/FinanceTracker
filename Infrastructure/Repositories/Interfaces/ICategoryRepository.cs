using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Infrastructure.Repositories.Interfaces;

public interface ICategoryRepository
{
  Task AddAsync(Category category);
  Task<List<Category>> GetAllByUserAsync(Guid userId);
  Task<bool> ExistsAsync(Guid id, Guid userId);
}
