namespace SecondHandShop.Shared.DTOs;

public record CartItemDto
{
  public int ProductId { get; init; }
  public string ProductName { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public decimal Price { get; init; }
  public string ImageUrl { get; init; } = string.Empty;
  public int Quantity { get; init; }
  public decimal Total => Price * Quantity;
}