using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs.Request;

public class UpdateTransactionRequest
{
  [Required]
  public Guid Id { get; set; }
  [Required]
  public string Type { get; set; } = string.Empty;
  [Required][Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Amount should be greater than 0")]
  public decimal? Amount { get; set; }
  public Guid? FromAccount { get; set; }
  public Guid? ToAccount { get; set; }
  public Guid? Category { get; set; }
  public string? Note { get; set; }
  public string? Date { get; set; }

  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    if (!string.IsNullOrEmpty(Date) && !DateTime.TryParse(Date, out _))
    {
      yield return new ValidationResult("Invalid date format", new[] { nameof(Date) });
    }

    var type = Type.ToLowerInvariant();

    if (type == "income")
    {
      if (ToAccount is null)
      {
        yield return new ValidationResult("Destination Account is required for income", new[] { nameof(ToAccount) });
      }
      if (FromAccount is not null)
      {
        yield return new ValidationResult("No need for Source Account in income", new[] { nameof(FromAccount) });
      }
    }

    if (type == "expense")
    {
      if (FromAccount is null)
      {
        yield return new ValidationResult("Source Account is required for expense", new[] { nameof(FromAccount) });
      }
      if (ToAccount is not null)
      {
        yield return new ValidationResult("No need for Destination Account in expense", new[] { nameof(ToAccount) });
      }
    }

    if (type == "transfer")
    {
      if (ToAccount is null)
      {
        yield return new ValidationResult("Destination Account is required for transfer", new[] { nameof(ToAccount) });
      }

      if (FromAccount is null)
      {
        yield return new ValidationResult("Source Account is required for transfer", new[] { nameof(FromAccount) });
      }

      if (FromAccount is not null && ToAccount is not null && FromAccount == ToAccount)
      {
        yield return new ValidationResult("Source Account and Destination Account cannot be the same", new[] { nameof(FromAccount), nameof(ToAccount) });
      }
    }
  }

}
