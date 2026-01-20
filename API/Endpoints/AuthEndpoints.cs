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
      var (success, error) = await authService.RegisterUserAsync(newUser);
      if (!success) return ApiResults.Conflict("Registeration Failed", error);
      return ApiResults.Created();
    });

    group.MapPost("/login", async (LoginUserRequest loginUser, IAuthService authService) =>
    {
      var (success, token) = await authService.LoginUserAsync(loginUser);
      if (!success) return ApiResults.Unauthorized("Login Failed", new {general = "Credentials are incorrect"});
      return ApiResults.Ok(new { accessToken = token });
    });

    return app;
  }
}
