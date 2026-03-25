using Microsoft.EntityFrameworkCore;
using SecondHandShop.Server.Data;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Models;

namespace SecondHandShop.Server.Repositories;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
  public async Task<Order> CreateOrderAsync(Order order)
  {
    using var transaction = await context.Database.BeginTransactionAsync();
    try
    {
      context.Orders.Add(order);
      await context.SaveChangesAsync();

      var cartItems = await context.CartItems
          .Where(ci => ci.UserId == order.UserId)
          .ToListAsync();

      if (cartItems.Any())
      {
        context.CartItems.RemoveRange(cartItems);
      }

      await context.SaveChangesAsync();
      await transaction.CommitAsync();

      return order;
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

  public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
  {
    return await context.Orders
        .Where(o => o.UserId == userId)
        .Include(o => o.OrderItems)
        .OrderByDescending(o => o.OrderDate)
        .ToListAsync();
  }

  public async Task<Order?> GetOrderByIdAsync(int id, string userId)
  {
    return await context.Orders
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
        .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
  }

  public async Task<bool> DeleteOrderAsync(int id, string userId)
  {
    var order = await context.Orders
        .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

    if (order == null) return false;

    context.Orders.Remove(order);
    return await context.SaveChangesAsync() > 0;
  }
}