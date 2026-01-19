using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Repositories.Interfaces;
using FinanceTracker.Infrasturecture.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
  private readonly AppDbContext _db;
  public AuthRepository(AppDbContext db) => _db = db;

  public Task<bool> EmailExistsAsync(string email) => _db.Users.AnyAsync(user => user.Email == email);
  
  public async Task AddUserAsync(User user)
  {
    _db.Users.Add(user);
    await _db.SaveChangesAsync();
  }

  public async Task<User?> GetUserByEmailAsync(string email)
  {
    return await _db.Users.FirstOrDefaultAsync(user => user.Email == email);
  }
}
