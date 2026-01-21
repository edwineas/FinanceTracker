namespace FinanceTracker.API.Responses;

public record ApiErrorResponse(
  string Title,
  object? Errors
);