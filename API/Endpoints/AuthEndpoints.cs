using FinanceTracker.API.Responses;
using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Application.Services.Interfaces;

namespace FinanceTracker.API.Endpoints;

public static class AuthEndpoints
{
  public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/auth");

    group.MapPost("/register", async (RegisterUserRequest newUser, IAuthService authService) =>
    {
      var (success, error, accessToken, refreshToken) = await authService.RegisterUserAsync(newUser);
      if (!success) return ApiResults.Conflict("Registeration Failed", error);
      return ApiResults.Created(new { accessToken, refreshToken });
    });

    group.MapPost("/login", async (LoginUserRequest loginUser, IAuthService authService) =>
    {
      var (success, accessToken, refreshToken) = await authService.LoginUserAsync(loginUser);
      if (!success) return ApiResults.Unauthorized("Login Failed", new {general = new[] {"Credentials are incorrect"}});
      return ApiResults.Ok(new { accessToken, refreshToken });
    });

    group.MapPost("/refresh-token", async (RefreshTokenRequest request, IAuthService authService) =>
    {
      var (success, accessToken) = await authService.RefreshTokenAsync(request.RefreshToken);
      if (!success) return ApiResults.Unauthorized("Unauthorized");
      return ApiResults.Ok(new { accessToken });
    });

    return app;
  }
}
