namespace SecondHandShop.Shared.DTOs;

public record RegisterUserDto(
    string Email,
    string Password,
    string FirstName,
    string LastName);