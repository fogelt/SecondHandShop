using SecondHandShop.Server.Interfaces;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Shared.Enums;
using SecondHandShop.Server.Models;

namespace SecondHandShop.Server.Services;

public class OrderService(
    IStripeService stripeService,
    IOrderRepository orderRepo,
    IProductsRepository productRepo) : IOrderService
{
  public async Task<OrderDto?> ConfirmAndCreateOrderAsync(string sessionId, string userId)
  {
    var session = await stripeService.GetSessionAsync(sessionId);

    if (session?.PaymentStatus != "paid") return null;

    var productIds = session.Metadata["ProductIds"]
        .Split(',')
        .Select(int.Parse)
        .ToList();

    var products = await productRepo.GetProductsByListAsync(productIds);

    var order = new Order
    {
      UserId = userId,
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

    var createdOrder = await orderRepo.CreateOrderAsync(order);
    await productRepo.MarkProductsAsSoldAsync(productIds);

    return createdOrder;
  }
}