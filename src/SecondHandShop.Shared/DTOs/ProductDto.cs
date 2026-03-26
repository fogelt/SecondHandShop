using System.ComponentModel.DataAnnotations;

namespace SecondHandShop.Shared.DTOs;

public record ProductDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Namn är obligatoriskt")]
    [StringLength(50, ErrorMessage = "Namnet är för långt")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beskrivning saknas")]
    [MinLength(10, ErrorMessage = "Beskrivningen måste vara minst 10 tecken")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pris måste anges")]
    [Range(1, 100000, ErrorMessage = "Priset måste vara mellan 1 och 100 000 kr")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Bild-URL krävs")]
    [Url(ErrorMessage = "Måste vara en giltig URL")]
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsSold { get; set; }
}