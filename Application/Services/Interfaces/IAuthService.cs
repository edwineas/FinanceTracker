using FinanceTracker.Application.DTOs.Request;

namespace FinanceTracker.Application.Services.Interfaces;

public interface IAuthService
{
  Task<(bool Success, object? Error)> RegisterUserAsync(RegisterUserRequest request);
  Task<(bool Success, string? Token)> LoginUserAsync(LoginUserRequest request);
}
