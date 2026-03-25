namespace SecondHandShop.Shared.DTOs;

public record OrderDto
{
  public int Id { get; init; }
  public DateTime OrderDate { get; init; }
  public decimal TotalPrice { get; init; }
  public string ShippingAddress { get; init; } = string.Empty;
  public List<OrderItemDto> Items { get; init; } = [];
}