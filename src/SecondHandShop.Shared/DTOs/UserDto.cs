namespace SecondHandShop.Shared.DTOs;

public class UserDto
{
  public string Id { get; set; } = string.Empty;
  public string? Email { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public IList<string> Roles { get; set; } = [];
}