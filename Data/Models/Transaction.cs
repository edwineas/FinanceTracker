namespace FinanceTracker.Data.Models;

public class Transaction
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public String Type { get; set; } = string.Empty;
  public decimal Amount { get; set; }
  public Guid? FromAccountId { get; set; }
  public Guid? ToAccountId { get; set; }
  public Guid? CategoryId { get; set; }
  public string? Note { get; set; }
  public DateTime Date { get; set; }
  public DateTime CreatedAt { get; set; }
}
