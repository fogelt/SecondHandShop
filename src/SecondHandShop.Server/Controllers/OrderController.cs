using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Models;
using System.Security.Claims;

namespace SecondHandShop.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrderController(IOrderRepository orderRepo) : ControllerBase
{
  private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? User.FindFirst("nameid")?.Value;

  [HttpPost("create")]
  public async Task<IActionResult> CreateOrder([FromBody] Order order)
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    order.UserId = userId;

    try
    {
      var result = await orderRepo.CreateOrderAsync(order);
      return Ok(result);
    }
    catch (Exception ex)
    {
      return BadRequest($"Fel: {ex.Message}");
    }
  }

  [HttpGet("my-orders")]
  public async Task<IActionResult> GetMyOrders()
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    var orders = await orderRepo.GetUserOrdersAsync(userId);
    return Ok(orders);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetOrderDetails(int id)
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    var order = await orderRepo.GetOrderByIdAsync(id, userId);
    return order == null ? NotFound("Ordern hittades inte.") : Ok(order);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> CancelOrder(int id)
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    var deleted = await orderRepo.DeleteOrderAsync(id, userId);
    return deleted ? NoContent() : BadRequest("Kunde inte ta bort ordern.");
  }

}