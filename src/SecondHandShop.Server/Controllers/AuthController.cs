using SecondHandShop.Server.Data;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Shared.Models;
using SecondHandShop.Server.Models;
using SecondHandShop.Server.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace SecondHandShop.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly ITokenService _tokenService;
  private readonly ApplicationDbContext _context;

  public AuthController(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      ITokenService tokenService,
      ApplicationDbContext context)
  {
    _userManager = userManager;
    _signInManager = signInManager;
    _tokenService = tokenService;
    _context = context;
  }

  [HttpPost("register")]
  public async Task<ActionResult<AuthResponseDto>> Register(RegisterUserDto dto)
  {
    var existingUser = await _userManager.FindByEmailAsync(dto.Email);
    if (existingUser != null) return Conflict("E-postadressen används redan.");

    var user = new ApplicationUser
    {
      UserName = dto.Email,
      Email = dto.Email,
      FirstName = dto.FirstName,
      LastName = dto.LastName
    };

    var result = await _userManager.CreateAsync(user, dto.Password);
    if (!result.Succeeded) return BadRequest(result.Errors);

    await _userManager.AddToRoleAsync(user, "User");

    return await CreateAuthResponse(user);
  }

  [HttpPost("login")]
  public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
  {
    var user = await _userManager.FindByEmailAsync(dto.Email);
    if (user == null) return Unauthorized("Felaktig e-postadress eller lösenord.");

    var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

    if (result.IsLockedOut) return Unauthorized("Kontot är låst. Försök igen senare.");
    if (!result.Succeeded) return Unauthorized("Felaktig e-postadress eller lösenord.");

    return await CreateAuthResponse(user);
  }

  [HttpPost("refresh")]
  public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] string refreshToken)
  {
    var storedToken = await _context.Set<RefreshToken>()
        .Include(rt => rt.User)
        .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

    if (storedToken == null || !storedToken.IsActive)
      return Unauthorized("Ogiltig eller utgången refresh token.");

    storedToken.IsRevoked = true;
    storedToken.RevokedAt = DateTime.UtcNow;

    return await CreateAuthResponse(storedToken.User);
  }

  [HttpPost("logout")]
  [Authorize]
  public async Task<IActionResult> Logout([FromBody] string refreshToken)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    var token = await _context.Set<RefreshToken>()
        .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

    if (token == null) return NotFound();
    token.IsRevoked = true;
    token.RevokedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return NoContent();
  }

  private async Task<AuthResponseDto> CreateAuthResponse(ApplicationUser user)
  {
    var roles = await _userManager.GetRolesAsync(user);

    var accessToken = _tokenService.CreateToken(user, roles);
    var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

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