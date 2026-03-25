using SecondHandShop.Server.Models;

namespace SecondHandShop.Server.Interfaces;

public interface IOrderRepository
{
  Task<Order> CreateOrderAsync(Order order);
  Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
  Task<Order?> GetOrderByIdAsync(int id, string userId);
  Task<bool> DeleteOrderAsync(int id, string userId);
}