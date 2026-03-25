namespace SecondHandShop.Shared.DTOs;

public record OrderDto
{
  public int Id { get; set; }
  public DateTime OrderDate { get; set; }
  public decimal TotalPrice { get; set; }
  public string OrderStatus { get; set; } = string.Empty;
  public string PaymentStatus { get; set; } = string.Empty;
  public string ShippingStreet { get; set; } = string.Empty;
  public string ShippingCity { get; set; } = string.Empty;
  public string ShippingZipCode { get; set; } = string.Empty;
  public List<OrderItemDto> OrderItems { get; set; } = [];
  public string? CustomerName { get; set; }
  public string? CustomerEmail { get; set; }
}