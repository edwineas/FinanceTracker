using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Infrastructure.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
  Task AddAsync(RefreshToken refreshToken);
  Task<RefreshToken?> GetByTokenAsync(string token);
  Task RevokeAsync(string token);
}
