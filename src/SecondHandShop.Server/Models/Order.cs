using SecondHandShop.Shared.Enums;

namespace SecondHandShop.Server.Models;

public class Order
{
  public int Id { get; set; }
  public DateTime OrderDate { get; set; } = DateTime.UtcNow;
  public string UserId { get; set; } = null!;
  public ApplicationUser User { get; set; } = null!;

  public string ShippingStreet { get; set; } = string.Empty;
  public string ShippingCity { get; set; } = string.Empty;
  public string ShippingZipCode { get; set; } = string.Empty;

  public decimal TotalPrice { get; set; }
  public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Väntar;
  public OrderStatus OrderStatus { get; set; } = OrderStatus.Mottagen;

  public List<OrderItem> OrderItems { get; set; } = [];
}