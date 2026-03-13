namespace SecondHandShop.Shared.DTOs;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpires,
    string Email,
    string FirstName,
    string LastName,
    IList<string> Roles);