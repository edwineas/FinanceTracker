namespace FinanceTracker.Application.DTOs.Response;

public class TransactionSummaryResponse
{
  public DateTime Date { get; set; }
  public string Type { get; set; } = string.Empty;
  public string Category { get; set; } = string.Empty;
  public decimal Amount { get; set; }
  public string Account { get; set; } = string.Empty;
}
