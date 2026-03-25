using Microsoft.AspNetCore.Identity;

namespace SecondHandShop.Server.Models;

public class ApplicationUser : IdentityUser
{
  public string FirstName { get; set; } = "";
  public string LastName { get; set; } = "";
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public List<CartItem> Cart { get; set; } = [];
  public List<Address> Addresses { get; set; } = [];
  public List<Order> Orders { get; set; } = [];
}