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
  [HttpPost("create")]
  public async Task<IActionResult> CreateOrder([FromBody] Order order)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null) return Unauthorized();

    order.UserId = userId;
    return Ok(await orderRepo.CreateOrderAsync(order));
  }

  [HttpGet("my-orders")]
  public async Task<IActionResult> GetMyOrders()
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Ok(await orderRepo.GetUserOrdersAsync(userId!));
  }

  [Authorize(Roles = "Admin")]
  [HttpGet("admin/all")]
  public async Task<IActionResult> GetAllOrders() => Ok(await orderRepo.GetAllOrdersAdminAsync());

  [HttpGet("{id}")]
  public async Task<IActionResult> GetDetails(int id)
  {
    var userId = User.IsInRole("Admin") ? null : User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var result = await orderRepo.GetOrderDetailsAsync(id, userId);
    return result == null ? NotFound() : Ok(result);
  }

  [Authorize(Roles = "Admin")]
  [HttpPut("admin/update-status/{id}")]
  public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status, [FromQuery] string? paymentStatus = null)
  {
    var success = await orderRepo.UpdateOrderStatusAsync(id, status, paymentStatus);
    return success ? Ok() : BadRequest("Kunde inte uppdatera ordern.");
  }
}