using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Server.Interfaces;
using Stripe;

namespace SecondHandShop.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/payments")]
public class PaymentController(IStripeService stripeService) : ControllerBase
{
  [HttpPost("create-checkout-session")]
  public async Task<IActionResult> CreateCheckoutSession([FromBody] OrderDto orderDto)
  {
    try
    {
      var session = await stripeService.CreateCheckoutSessionAsync(orderDto);
      return Ok(new { Url = session.Url });
    }
    catch (StripeException e)
    {
      return BadRequest(new { message = e.Message });
    }
  }
}