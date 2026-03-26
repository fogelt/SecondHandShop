using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Server.Interfaces;

public interface IOrderService
{
  Task<OrderDto?> ConfirmAndCreateOrderAsync(string sessionId, string userId);
}