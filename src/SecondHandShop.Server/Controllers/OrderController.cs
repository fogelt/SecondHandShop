using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Models;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Shared.Enums;
using Stripe.Checkout;
using Stripe;
using System.Security.Claims;

namespace SecondHandShop.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrderController(IOrderRepository orderRepo, IProductsRepository productRepo, IConfiguration config) : ControllerBase
{
  [HttpGet("confirm-payment/{sessionId}")]
  public async Task<IActionResult> ConfirmPayment(string sessionId)
  {
    StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
    var session = await new SessionService().GetAsync(sessionId);

    if (session?.PaymentStatus != "paid")
      return BadRequest("Betalningen kunde inte verifieras.");

    var m = session.Metadata;
    var productIds = m["ProductIds"].Split(',').Select(int.Parse).ToList();

    var products = await productRepo.GetProductsByListAsync(productIds);

    var order = MapSessionToOrder(session, products);

    var result = await orderRepo.CreateOrderAsync(order);
    await productRepo.MarkProductsAsSoldAsync(productIds);

    return Ok(result);
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

  private Order MapSessionToOrder(Session session, List<Models.Product> products)
  {
    return new Order
    {
      UserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
      OrderDate = DateTime.Now,
      TotalPrice = products.Sum(p => p.Price),
      ShippingStreet = session.Metadata["Street"],
      ShippingCity = session.Metadata["City"],
      ShippingZipCode = session.Metadata["Zip"],
      OrderStatus = OrderStatus.Mottagen,
      PaymentStatus = PaymentStatus.Betald,
      OrderItems = products.Select(p => new OrderItem
      {
        ProductId = p.Id,
        Quantity = 1,
        PriceAtPurchase = p.Price
      }).ToList()
    };
  }
}