using System.Security.Claims;
using FinanceTracker.API.Responses;
using FinanceTracker.Application.Services.Interfaces;

namespace FinanceTracker.API.Endpoints;

public static class DashboardEndpoints
{
  public static IEndpointRouteBuilder MapDashboardEndpoints (this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/dashboard").RequireAuthorization();

    group.MapGet("/summary", async (ClaimsPrincipal user, IDashboardService dashboardService) =>
    {
      var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

      var (totalBalance, totalIncome, totalExpence, netSaving) = await dashboardService.getSummary(userId);

      return ApiResults.Ok(new {totalBalance, totalIncome, totalExpence, netSaving}, "Ok");
    });

    return app;
  }
}
