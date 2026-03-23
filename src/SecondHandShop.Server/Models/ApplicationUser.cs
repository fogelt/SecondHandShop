using Microsoft.AspNetCore.Identity;
using SecondHandShop.Server.Models;

namespace SecondHandShop.Shared.Models;

public class ApplicationUser : IdentityUser
{
  public string FirstName { get; set; } = "";
  public string LastName { get; set; } = "";
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public List<CartItem> Cart = [];
}