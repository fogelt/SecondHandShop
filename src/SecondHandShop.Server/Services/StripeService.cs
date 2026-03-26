using Stripe;
using Stripe.Checkout;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Server.Interfaces;

namespace SecondHandShop.Server.Services;

public class StripeService : IStripeService
{
  private readonly IConfiguration _config;

  public StripeService(IConfiguration config)
  {
    _config = config;
    StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
  }

  public async Task<Session> CreateCheckoutSessionAsync(OrderDto orderDto)
  {
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
          },
        },
        Quantity = 1,
      }).ToList(),
      Mode = "payment",
      Metadata = new Dictionary<string, string>
            {
                { "Street", orderDto.ShippingStreet },
                { "City", orderDto.ShippingCity },
                { "Zip", orderDto.ShippingZipCode },
                { "ProductIds", string.Join(",", orderDto.OrderItems.Select(i => i.ProductId)) }
            },
      SuccessUrl = $"{_config["AppUrl"]}/order-success/{{CHECKOUT_SESSION_ID}}",
      CancelUrl = $"{_config["AppUrl"]}/payment-cancelled",
    };

    var service = new SessionService();
    return await service.CreateAsync(options);
  }

  public async Task<Session> GetSessionAsync(string sessionId)
  {
    var service = new SessionService();
    return await service.GetAsync(sessionId);
  }
}