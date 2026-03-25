using System.ComponentModel.DataAnnotations;

namespace SecondHandShop.Server.Models;

public class CartItem
{
  [Key]
  public int Id { get; set; }

  [Required]
  public string UserId { get; set; } = null!;
  public ApplicationUser User { get; set; } = null!;

  [Required]
  public int ProductId { get; set; }
  public Product Product { get; set; } = null!;

  public int Quantity { get; set; } = 1;
  public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}