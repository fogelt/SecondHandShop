namespace SecondHandShop.Server.Models;

public class Order
{
  public int Id { get; set; }
  public DateTime OrderDate { get; set; } = DateTime.UtcNow;

  public string UserId { get; set; } = null!;
  public ApplicationUser User { get; set; } = null!;

  public string ShippingAddress { get; set; } = string.Empty;

  public decimal TotalPrice { get; set; }
  public List<OrderItem> OrderItems { get; set; } = [];
}