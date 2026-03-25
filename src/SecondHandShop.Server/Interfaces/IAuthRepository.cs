using Microsoft.AspNetCore.Identity;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Server.Models;

namespace SecondHandShop.Server.Interfaces;

public interface IAuthRepository
{
  Task<IEnumerable<UserDto>> GetAllUsersAsync();
  Task<(IdentityResult Result, AuthResponseDto? Response)> UpdateUserAsync(string id, UpdateUserDto model);
  Task<IdentityResult> UpdateUserRoleAsync(string userId, string newRole);
  Task<IdentityResult> DeleteUserAsync(string id);
  Task<IdentityResult> RegisterAsync(RegisterRequestDto dto);
  Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
  Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
  Task<bool> LogoutAsync(string refreshToken, string userId);
  Task<ApplicationUser?> GetUserByEmailAsync(string email);
}