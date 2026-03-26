using System.ComponentModel.DataAnnotations;

namespace SecondHandShop.Shared.DTOs;

public record UpdateUserDto
{
  public string Id { get; set; } = string.Empty;
  [Required(ErrorMessage = "Förnamn krävs")]
  public string FirstName { get; set; } = string.Empty;
  [Required(ErrorMessage = "Efternamn krävs")]
  public string LastName { get; set; } = string.Empty;
  [Required(ErrorMessage = "E-post krävs")]
  [EmailAddress(ErrorMessage = "Ogiltig e-postadress")]
  public string Email { get; set; } = string.Empty;
  public string? NewPassword { get; set; }
  [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
  public string? ConfirmPassword { get; set; }
  public AddressRegistrationDto Address { get; set; } = new();
}