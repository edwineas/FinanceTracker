using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs.Request;
public class LoginUserRequest
{
  [Required][EmailAddress]
  public string Email { get; set; } = string.Empty;
  [Required]
  public string Password { get; set; } = string.Empty;
}
