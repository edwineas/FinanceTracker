using FinanceTracker.Application.DTOs.Request;

namespace FinanceTracker.Application.Services.Interfaces;

public interface IAuthService
{
  Task<(bool Success, object? Error, string? Token)> RegisterUserAsync(RegisterUserRequest request);
  Task<(bool Success, string? Token)> LoginUserAsync(LoginUserRequest request);
}
