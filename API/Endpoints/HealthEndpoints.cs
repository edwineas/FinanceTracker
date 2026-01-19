namespace FinanceTracker.API.Endpoints;

public static class HealthEndpoints
{
  public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
  {
    
    app.MapGet("/", () => "Hello from Project");

    app.MapGet("/health", () => new { status = "Healthy", timestamp = DateTime.UtcNow });

    return app;
  }
}
