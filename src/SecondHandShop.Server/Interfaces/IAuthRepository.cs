using Microsoft.AspNetCore.Identity;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Shared.Models;

namespace SecondHandShop.Server.Interfaces;

public interface IAuthRepository
{
  Task<IdentityResult> RegisterAsync(RegisterUserDto dto);
  Task<AuthResponseDto?> LoginAsync(LoginUserDto dto);
  Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
  Task<bool> LogoutAsync(string refreshToken, string userId);
  Task<ApplicationUser?> GetUserByEmailAsync(string email);
}