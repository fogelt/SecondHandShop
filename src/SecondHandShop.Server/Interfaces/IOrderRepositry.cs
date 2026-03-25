using SecondHandShop.Server.Models;

namespace SecondHandShop.Server.Interfaces;

public interface IOrderRepository
{
  Task<Order?> GetOrderByIdAsync(int id, string userId);
  Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
  Task<Order> CreateOrderAsync(Order order);
  Task<bool> DeleteOrderAsync(int id, string userId);
}