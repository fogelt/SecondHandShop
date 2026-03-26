using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondHandShop.Server.Interfaces;
using System.Security.Claims;

namespace SecondHandShop.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrderController(IOrderService orderService, IOrderRepository orderRepo) : ControllerBase
{
  [HttpGet("confirm-payment/{sessionId}")]
  public async Task<IActionResult> ConfirmPayment(string sessionId)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    var result = await orderService.ConfirmAndCreateOrderAsync(sessionId, userId!);

    return result == null
        ? BadRequest("Betalningen kunde inte verifieras.")
        : Ok(result);
  }

  [HttpGet("my-orders")]
  public async Task<IActionResult> GetMyOrders()
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Ok(await orderRepo.GetUserOrdersAsync(userId!));
  }

  [Authorize(Roles = "Admin")]
  [HttpGet("admin/all")]
  public async Task<IActionResult> GetAllOrders() =>
      Ok(await orderRepo.GetAllOrdersAdminAsync());

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