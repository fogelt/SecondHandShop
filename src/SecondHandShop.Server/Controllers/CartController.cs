using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Shared.DTOs;
using System.Security.Claims;

namespace SecondHandShop.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController(ICartRepository cartRepo) : ControllerBase
{
  private readonly ICartRepository _cartRepo = cartRepo;

  [HttpGet]
  public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCart()
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    return Ok(await _cartRepo.GetUserCartAsync(userId!));
  }

  [HttpPost("add/{productId}")]
  public async Task<IActionResult> Add(int productId)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var success = await _cartRepo.AddToCartAsync(userId!, productId);
    return success ? Ok() : BadRequest("Could not add item to cart.");
  }

  [HttpDelete("{productId}")]
  public async Task<IActionResult> Remove(int productId)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var success = await _cartRepo.RemoveFromCartAsync(userId!, productId);
    return success ? Ok() : NotFound();
  }
}