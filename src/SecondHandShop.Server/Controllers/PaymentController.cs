using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondHandShop.Shared.DTOs;
using Stripe;
using Stripe.Checkout;

namespace SecondHandShop.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/payments")]
public class PaymentController(IConfiguration config) : ControllerBase
{
  [HttpPost("create-checkout-session")]
  public async Task<IActionResult> CreateCheckoutSession([FromBody] OrderDto orderDto)
  {
    StripeConfiguration.ApiKey = config["Stripe:SecretKey"];

    var options = new SessionCreateOptions
    {
      PaymentMethodTypes = new List<string> { "card" },
      LineItems = orderDto.OrderItems.Select(item => new SessionLineItemOptions
      {
        PriceData = new SessionLineItemPriceDataOptions
        {
          UnitAmount = (long)(item.UnitPrice * 100),
          Currency = "sek",
          ProductData = new SessionLineItemPriceDataProductDataOptions
          {
            Name = item.ProductName,
            Images = new List<string> { item.ProductImageUrl }
          },
        },
        Quantity = item.Quantity,
      }).ToList(),
      Mode = "payment",

      Metadata = new Dictionary<string, string> { { "OrderData", System.Text.Json.JsonSerializer.Serialize(orderDto) } },
      SuccessUrl = $"{config["AppUrl"]}/order-success/{{CHECKOUT_SESSION_ID}}",
      CancelUrl = $"{config["AppUrl"]}/payment-cancelled",
    };

    var service = new SessionService();
    Session session = await service.CreateAsync(options);

    return Ok(new { Url = session.Url });
  }


}