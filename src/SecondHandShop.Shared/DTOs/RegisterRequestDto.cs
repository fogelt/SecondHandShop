using System.ComponentModel.DataAnnotations;

namespace SecondHandShop.Shared.DTOs;

public record RegisterRequestDto
{
    [Required(ErrorMessage = "E-post krävs")]
    [EmailAddress(ErrorMessage = "Ogiltig e-postadress")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lösenord krävs")]
    [MinLength(8, ErrorMessage = "Lösenordet måste vara minst 8 tecken")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Förnamn krävs")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Efternamn krävs")]
    public string LastName { get; set; } = string.Empty;
}