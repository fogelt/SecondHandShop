namespace SecondHandShop.Server.Models;

public class Address
{
  public int Id { get; set; }
  public string Street { get; set; } = string.Empty;
  public string City { get; set; } = string.Empty;
  public string ZipCode { get; set; } = string.Empty;
  public string Country { get; set; } = string.Empty;

  public string UserId { get; set; } = null!;
}