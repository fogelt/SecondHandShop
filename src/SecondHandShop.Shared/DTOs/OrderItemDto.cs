namespace SecondHandShop.Shared.DTOs;

public record OrderItemDto
{
  public int ProductId { get; set; }
  public string ProductName { get; set; } = string.Empty;
  public string ProductImageUrl { get; set; } = string.Empty;
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
}