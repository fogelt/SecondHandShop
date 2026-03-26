using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Models;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Shared.Enums;
using System.Security.Claims;

namespace SecondHandShop.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrderController(IOrderRepository orderRepo, IConfiguration config) : ControllerBase
{
  [HttpGet("confirm-payment/{sessionId}")]
  public async Task<IActionResult> ConfirmPayment(string sessionId)
  {
    Stripe.StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
    var service = new Stripe.Checkout.SessionService();
    var session = await service.GetAsync(sessionId);

    if (session != null && session.PaymentStatus == "paid")
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var jsonData = session.Metadata["OrderData"];
      var dto = System.Text.Json.JsonSerializer.Deserialize<OrderDto>(jsonData);

      if (dto != null)
      {
        var order = new Order
        {
          UserId = userId!,
          OrderDate = DateTime.Now,
          TotalPrice = dto.TotalPrice,
          ShippingStreet = dto.ShippingStreet,
          ShippingCity = dto.ShippingCity,
          ShippingZipCode = dto.ShippingZipCode,
          OrderStatus = OrderStatus.Mottagen,
          PaymentStatus = PaymentStatus.Betald,
          OrderItems = dto.OrderItems.Select(oi => new OrderItem
          {
            ProductId = oi.ProductId,
            Quantity = oi.Quantity,
            PriceAtPurchase = oi.UnitPrice
          }).ToList()
        };

        var result = await orderRepo.CreateOrderAsync(order);
        return Ok(result);
      }
    }

    return BadRequest("Betalning kunde inte verifieras.");
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