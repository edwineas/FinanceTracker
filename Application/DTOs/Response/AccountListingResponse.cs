namespace FinanceTracker.Application.DTOs.Response;

public class AccountListingResponse
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
}
