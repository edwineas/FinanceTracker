namespace FinanceTracker.Data.Models;

public class Account
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
}
