using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Data;
using FinanceTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
  private readonly AppDbContext _db;

  public RefreshTokenRepository(AppDbContext db) => _db = db;

  public async Task AddAsync(RefreshToken refreshToken)
  {
    _db.RefreshTokens.Add(refreshToken);
    await _db.SaveChangesAsync();
  }

  public async Task<RefreshToken?> GetByTokenAsync(string token) => await _db.RefreshTokens.FirstOrDefaultAsync(refreshToken => refreshToken.Token == token && !refreshToken.IsRevoked);

  public async Task RevokeAsync(string token)
  {
    var refreshToken = await GetByTokenAsync(token);
    if (refreshToken != null)
    {
      refreshToken.IsRevoked = true;
      await _db.SaveChangesAsync();
    }
  }
}
