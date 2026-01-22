using FinanceTracker.Application.DTOs.Request;

namespace FinanceTracker.Application.Services.Interfaces;

public interface IAuthService
{
  Task<(bool Success, object? Error, string? AccessToken, string? RefreshToken)> RegisterUserAsync(RegisterUserRequest request);
  Task<(bool Success, string? AccessToken, string? RefreshToken)> LoginUserAsync(LoginUserRequest request);
  Task<(bool Success, string? AccessToken)> RefreshTokenAsync(string refreshToken);
}
