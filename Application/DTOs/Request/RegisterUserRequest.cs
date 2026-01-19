using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs.Request;
public class RegisterUserRequest
{
  [Required]
  public string Name { get; set; } = string.Empty;
  [Required][EmailAddress]
  public string Email { get; set; } = string.Empty;
  [Required][MinLength(6, ErrorMessage ="Password must be at least 6 characters long")]
  public string Password { get; set; } = string.Empty;
  [Required(ErrorMessage ="The Confirmation Password is required")][Compare("Password", ErrorMessage ="The Password didn't match")]
  public string ConfirmPassword { get; set; } = string.Empty;
}
