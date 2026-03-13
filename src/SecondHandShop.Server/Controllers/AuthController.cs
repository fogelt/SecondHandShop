using SecondHandShop.Server.Data;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Shared.Models;
using SecondHandShop.Server.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace SecondHandShop.Controllers;

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
    // TODO: Implementera registrering
    // 1. Kontrollera att e-postadressen inte redan finns med FindByEmailAsync
    //    — returnera Conflict("E-postadressen används redan.") om den finns
    // 2. Skapa ett ApplicationUser-objekt med UserName = Email
    // 3. Anropa _userManager.CreateAsync(user, dto.Password)
    // 4. Om !result.Succeeded — returnera BadRequest med result.Errors
    // 5. Lägg till rollen "User" med AddToRoleAsync (rollen skapas i Program.cs)
    // 6. Hämta roller med GetRolesAsync
    // 7. Generera access token med _tokenService.GenerateAccessToken
    // 8. Skapa refresh token med _tokenService.CreateRefreshTokenAsync
    // 9. Returnera Ok med ett nytt AuthResponseDto-objekt

    throw new NotImplementedException();
  }

  [HttpPost("login")]
  public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
  {
    // TODO: Implementera inloggning
    // 1. Hitta användaren med FindByEmailAsync
    //    — returnera Unauthorized("Felaktig e-postadress eller lösenord.") om null
    //    (VIKTIGT: samma meddelande för fel e-post OCH fel lösenord — undviker user enumeration)
    // 2. Anropa _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true)
    // 3. Kontrollera result.IsLockedOut — returnera Unauthorized med lockout-meddelande
    // 4. Kontrollera !result.Succeeded — returnera Unauthorized
    // 5. Hämta roller, generera tokens och returnera AuthResponseDto
    //    (samma struktur som i Register)

    throw new NotImplementedException();
  }

  [HttpPost("refresh")]
  public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] string refreshToken)
  {
    // TODO: Implementera token-refresh
    // 1. Hitta refresh token i databasen med Include(rt => rt.User)
    //    Använd: _context.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == refreshToken)
    // 2. Kontrollera att token finns och är aktiv (token.IsActive)
    //    — returnera Unauthorized("Ogiltig eller utgången refresh token.") annars
    // 3. Revocera den gamla token: token.IsRevoked = true, token.RevokedAt = DateTime.UtcNow
    // 4. Hämta användaren, generera nya tokens
    // 5. Spara ändringarna med SaveChangesAsync
    // 6. Returnera AuthResponseDto med nya tokens

    throw new NotImplementedException();
  }

  [HttpPost("logout")]
  [Authorize]
  public async Task<IActionResult> Logout([FromBody] string refreshToken)
  {
    // TODO: Implementera logout
    // 1. Hämta inloggad användares ID ur claims:
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    // 2. Hitta refresh token i databasen som tillhör denna användare
    //    (kontrollera både Token och UserId för säkerhet)
    // 3. Returnera NotFound om token inte hittas
    // 4. Revocera token och spara
    // 5. Returnera NoContent()

    throw new NotImplementedException();
  }
}