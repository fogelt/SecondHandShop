namespace SecondHandShop.Shared.DTOs;

public record UpdateUserDto
{
  public string Id { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string? NewPassword { get; set; }

  [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
  public string? ConfirmPassword { get; set; }
}