using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecondHandShop.Server.Data;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Models;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Server.Repositories;

public class AuthRepository(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService,
    ApplicationDbContext context) : IAuthRepository
{

  public async Task<IdentityResult> DeleteUserAsync(string id)
  {
    var user = await userManager.FindByIdAsync(id);
    if (user == null)
    {
      return IdentityResult.Failed(new IdentityError { Description = "User not found." });
    }
    return await userManager.DeleteAsync(user);
  }

  public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
      => await userManager.FindByEmailAsync(email);

  public async Task<IdentityResult> RegisterAsync(RegisterRequestDto dto)
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

  public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
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

  // USER FUNCTIONS //
  public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
  {
    var users = await userManager.Users.ToListAsync();
    var userDtos = new List<UserDto>();

    foreach (var user in users)
    {
      userDtos.Add(new UserDto
      {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Roles = await userManager.GetRolesAsync(user)
      });
    }
    return userDtos;
  }

  public async Task<IdentityResult> UpdateUserRoleAsync(string userId, string newRole)
  {
    var user = await userManager.FindByIdAsync(userId);
    if (user == null)
      return IdentityResult.Failed(new IdentityError { Description = "User not found." });

    var currentRoles = await userManager.GetRolesAsync(user);

    var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
    if (!removeResult.Succeeded) return removeResult;

    return await userManager.AddToRoleAsync(user, newRole);
  }

  public async Task<(IdentityResult Result, AuthResponseDto? Response)> UpdateUserAsync(string id, UpdateUserDto model)
  {
    var user = await userManager.FindByIdAsync(id);
    if (user == null)
      return (IdentityResult.Failed(new IdentityError { Description = "User not found" }), null);

    user.FirstName = model.FirstName;
    user.LastName = model.LastName;
    user.Email = model.Email;
    user.UserName = model.Email;

    var result = await userManager.UpdateAsync(user);

    if (result.Succeeded && !string.IsNullOrWhiteSpace(model.NewPassword))
    {
      var token = await userManager.GeneratePasswordResetTokenAsync(user);
      result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);
    }

    if (result.Succeeded)
    {
      var authResponse = await CreateAuthResponse(user);
      return (result, authResponse);
    }

    return (result, null);
  }
}