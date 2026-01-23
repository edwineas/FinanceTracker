namespace FinanceTracker.Application.DTOs.Response;

public class AccountWithBalanceResponse
{
  public string Name { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public decimal Balance { get; set; }
  public DateTime CreatedAt { get; set; }
}
