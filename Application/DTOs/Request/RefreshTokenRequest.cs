using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs.Request;

public class RefreshTokenRequest
{
  [Required]
  public string RefreshToken { get; set; } = string.Empty;
}
