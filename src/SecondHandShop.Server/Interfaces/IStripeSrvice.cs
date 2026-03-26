using SecondHandShop.Shared.DTOs;
using Stripe.Checkout;

namespace SecondHandShop.Server.Interfaces;

public interface IStripeService
{
  Task<Session> CreateCheckoutSessionAsync(OrderDto orderDto);
  Task<Session> GetSessionAsync(string sessionId);
}