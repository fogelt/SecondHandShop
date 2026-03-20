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
  public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
  {
    if (await authRepo.GetUserByEmailAsync(request.Email) != null)
      return Conflict("E-postadressen används redan.");

    var result = await authRepo.RegisterAsync(request);
    if (!result.Succeeded) return BadRequest(result.Errors);

    var loginRequest = new LoginRequestDto { Email = request.Email, Password = request.Password };
    return await Login(loginRequest);
  }

  [HttpPost("login")]
  public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
  {
    var response = await authRepo.LoginAsync(dto);
    return response == null ? Unauthorized("Felaktig e-post eller lösenord.") : Ok(response);
  }

  [HttpDelete("delete/{id}")]
  [Authorize]
  public async Task<IActionResult> Delete(string id)
  {
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var isAdmin = User.IsInRole("Admin");
    if (currentUserId != id && !isAdmin)
    {
      return Forbid("Du har inte behörighet att ta bort detta konto.");
    }

    var result = await authRepo.DeleteUserAsync(id);

    if (!result.Succeeded)
      return BadRequest(result.Errors);

    return NoContent();
  }

  [HttpPost("update-role")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto dto)
  {
    var result = await authRepo.UpdateUserRoleAsync(dto.UserId, dto.NewRole);

    if (!result.Succeeded)
      return BadRequest(result.Errors);

    return Ok(new { message = $"Användarens roll har uppdaterats till {dto.NewRole}" });
  }

  [HttpPut("update-user/{id}")]
  [Authorize]
  public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto model)
  {
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (currentUserId != id)
    {
      return Forbid("Du har inte behörighet att uppdatera detta konto.");
    }
    if (!ModelState.IsValid)
      return BadRequest(ModelState);
    var result = await authRepo.UpdateUserAsync(id, model);
    if (!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }
    return Ok(new { Message = "Profilen har uppdaterats!" });
  }

  [HttpGet("get-all")]
  [Authorize(Roles = "Admin")]
  public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
  {
    var users = await authRepo.GetAllUsersAsync();
    return Ok(users);
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