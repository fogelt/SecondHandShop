namespace SecondHandShop.Shared.DTOs;

public record AddressDto
{
  public int Id { get; init; }
  public string Street { get; init; } = string.Empty;
  public string City { get; init; } = string.Empty;
  public string ZipCode { get; init; } = string.Empty;
  public string Country { get; init; } = string.Empty;
}