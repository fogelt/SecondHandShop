namespace SecondHandShop.Shared.DTOs;

public record OrderItemDto
{
  public int ProductId { get; init; }
  public string ProductName { get; init; } = string.Empty;
  public int Quantity { get; init; }
  public decimal PriceAtPurchase { get; init; }
}