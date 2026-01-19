using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs.Request;
public class CreateAccountRequest
{
  [Required]
  public string Name { get; set; } = string.Empty;
  [Required]
  public string Type { get; set; } = string.Empty;
}
