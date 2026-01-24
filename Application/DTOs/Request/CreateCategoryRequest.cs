using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs.Request;
public class CreateCategoryRequest
{
  [Required][MinLength(2)]
  public string Name { get; set; } = string.Empty;
}
