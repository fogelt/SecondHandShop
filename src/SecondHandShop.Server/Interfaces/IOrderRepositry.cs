using SecondHandShop.Shared.DTOs;
using SecondHandShop.Server.Models;

namespace SecondHandShop.Server.Interfaces;

public interface IOrderRepository
{
  Task<OrderDto> CreateOrderAsync(Order order);
  Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
  Task<IEnumerable<OrderDto>> GetAllOrdersAdminAsync();
  Task<OrderDto?> GetOrderDetailsAsync(int id, string? userId = null);
  Task<bool> DeleteOrderAsync(int id, string userId);
  Task<bool> UpdateOrderStatusAsync(int id, string status, string? paymentStatus = null);
}