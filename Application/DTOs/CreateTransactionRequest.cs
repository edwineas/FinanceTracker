using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs;
public class CreateTransactionRequest : IValidatableObject
{
  [Required]
  public string Type { get; set; } = string.Empty;

  [Required][Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Amount should be grater than 0")]
  public decimal? Amount { get; set; }
  public Guid? FromAccountId { get; set; }
  public Guid? ToAccountId { get; set; }
  public Guid? CategoryId { get; set; }
  public string? Note { get; set; }
  public DateTime? Date { get; set; }

  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    var type = Type.ToLowerInvariant();

    if (type == "income")
    {
      if (ToAccountId is null)
      {
        yield return new ValidationResult("Destination Account is required for income", new[] { nameof(ToAccountId) });
      }
      if (FromAccountId is not null)
      {
        yield return new ValidationResult("No need for Source Account in income", new[] { nameof(FromAccountId) });
      }
    }

    if (type == "expense")
    {
      if (FromAccountId is null)
      {
        yield return new ValidationResult("Source Account is required for expense", new[] { nameof(FromAccountId) });
      }
      if (ToAccountId is not null)
      {
        yield return new ValidationResult("No need for Destination Account in expense", new[] { nameof(ToAccountId) });
      }
    }

    if (type == "transfer")
    {
      if (ToAccountId is null)
      {
        yield return new ValidationResult("Destination Account is required for transfer", new[] { nameof(ToAccountId) });
      }

      if (FromAccountId is null)
      {
        yield return new ValidationResult("Source Account is required for transfer", new[] { nameof(FromAccountId) });
      }

      if (FromAccountId is not null && ToAccountId is not null && FromAccountId == ToAccountId)
      {
        yield return new ValidationResult("Source Account and Destination Account cannot be the same", new[] { nameof(FromAccountId), nameof(ToAccountId) });
      }
    }
  }
}
