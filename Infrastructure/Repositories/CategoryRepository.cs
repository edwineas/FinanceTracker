using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Repositories.Interfaces;
using FinanceTracker.Infrasturecture.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
  private readonly AppDbContext _db;

  public CategoryRepository(AppDbContext db) => _db = db;

  public async Task AddAsync(Category category)
  {
    _db.Categories.Add(category);
    await _db.SaveChangesAsync();
  }

  public async Task<List<Category>> GetAllByUserAsync(Guid userId) => await _db.Categories.Where(category => category.UserId == userId).ToListAsync();
}
