namespace FinanceTracker.API.Responses;

public record ApiResponse<T>(
  string Title,
  T? Data
);