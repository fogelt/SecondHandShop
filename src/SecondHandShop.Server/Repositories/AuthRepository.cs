using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecondHandShop.Server.Data;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Models;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Shared.Models;

namespace SecondHandShop.Server.Repositories;

public class AuthRepository(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService,
    ApplicationDbContext context) : IAuthRepository
{
  public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
      => await userManager.FindByEmailAsync(email);

  public async Task<IdentityResult> RegisterAsync(RegisterUserDto dto)
  {
    var user = new ApplicationUser
    {
      UserName = dto.Email,
      Email = dto.Email,
      FirstName = dto.FirstName,
      LastName = dto.LastName
    };

    var result = await userManager.CreateAsync(user, dto.Password);
    if (result.Succeeded)
    {
      await userManager.AddToRoleAsync(user, "User");
    }
    return result;
  }

  public async Task<AuthResponseDto?> LoginAsync(LoginUserDto dto)
  {
    var user = await userManager.FindByEmailAsync(dto.Email);
    if (user == null) return null;

    var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, true);
    if (!result.Succeeded) return null;

    return await CreateAuthResponse(user);
  }

  public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
  {
    var storedToken = await context.Set<RefreshToken>()
        .Include(rt => rt.User)
        .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

    if (storedToken == null || !storedToken.IsActive) return null;

    storedToken.IsRevoked = true;
    storedToken.RevokedAt = DateTime.UtcNow;
    await context.SaveChangesAsync();

    return await CreateAuthResponse(storedToken.User);
  }

  public async Task<bool> LogoutAsync(string refreshToken, string userId)
  {
    var token = await context.Set<RefreshToken>()
        .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

    if (token == null) return false;

    token.IsRevoked = true;
    token.RevokedAt = DateTime.UtcNow;
    return await context.SaveChangesAsync() > 0;
  }

  private async Task<AuthResponseDto> CreateAuthResponse(ApplicationUser user)
  {
    var roles = await userManager.GetRolesAsync(user);
    var accessToken = tokenService.CreateToken(user, roles);
    var refreshToken = await tokenService.CreateRefreshTokenAsync(user);

    return new AuthResponseDto(
        accessToken,
        refreshToken.Token,
        DateTime.UtcNow.AddMinutes(15),
        user.Email!,
        user.FirstName,
        user.LastName,
        roles
    );
  }
}