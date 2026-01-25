namespace FinanceTracker.Application.DTOs.Response;

public class TransactionListingResponse
{
  public Guid Id { get; set; }
  public DateTime Date { get; set; }
  public string Type { get; set; } = string.Empty;
  public string Category { get; set; } = string.Empty;
  public Guid? CategoryId { get; set; }
  public decimal Amount { get; set; }
  public string FromAccount { get; set; } = string.Empty;
  public Guid? FromAccountId { get; set; }
  public string ToAccount { get; set; } = string.Empty;
  public Guid? ToAccountId { get; set; }
  public string Note { get; set; } = string.Empty;
}
