using SecondHandShop.Shared.DTOs;
using SecondHandShop.Server.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SecondHandShop.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthRepository authRepo) : ControllerBase
{
  [HttpPost("register")]
  public async Task<ActionResult<AuthResponseDto>> Register(RegisterUserDto dto)
  {
    if (await authRepo.GetUserByEmailAsync(dto.Email) != null)
      return Conflict("E-postadressen används redan.");

    var result = await authRepo.RegisterAsync(dto);
    if (!result.Succeeded) return BadRequest(result.Errors);

    var loginDto = new LoginUserDto { Email = dto.Email, Password = dto.Password };
    return await Login(loginDto);
  }

  [HttpPost("login")]
  public async Task<ActionResult<AuthResponseDto>> Login(LoginUserDto dto)
  {
    var response = await authRepo.LoginAsync(dto);
    return response == null ? Unauthorized("Felaktig e-post eller lösenord.") : Ok(response);
  }

  [HttpPost("refresh")]
  public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] string refreshToken)
  {
    var response = await authRepo.RefreshTokenAsync(refreshToken);
    return response == null ? Unauthorized("Ogiltig token.") : Ok(response);
  }

  [HttpPost("logout")]
  [Authorize]
  public async Task<IActionResult> Logout([FromBody] string refreshToken)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var success = await authRepo.LogoutAsync(refreshToken, userId!);
    return success ? NoContent() : NotFound();
  }
}