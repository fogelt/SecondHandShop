using Microsoft.EntityFrameworkCore;
using SecondHandShop.Server.Data;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Models;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Shared.Enums;

namespace SecondHandShop.Server.Repositories;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
  public async Task<OrderDto> CreateOrderAsync(Order order)
  {
    using var transaction = await context.Database.BeginTransactionAsync();
    try
    {
      context.Orders.Add(order);
      await context.SaveChangesAsync();

      var cartItems = await context.CartItems
          .Where(ci => ci.UserId == order.UserId)
          .ToListAsync();

      if (cartItems.Any()) context.CartItems.RemoveRange(cartItems);

      await context.SaveChangesAsync();
      await transaction.CommitAsync();

      var savedOrder = await context.Orders
          .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
          .FirstAsync(o => o.Id == order.Id);

      return MapToDto(savedOrder);
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

  public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
  {
    return await context.Orders
        .Where(o => o.UserId == userId)
        .Include(o => o.OrderItems)
        .OrderByDescending(o => o.OrderDate)
        .Select(o => MapToDto(o))
        .ToListAsync();
  }

  public async Task<IEnumerable<OrderDto>> GetAllOrdersAdminAsync()
  {
    return await context.Orders
        .Include(o => o.User)
        .Include(o => o.OrderItems)
        .OrderByDescending(o => o.OrderDate)
        .Select(o => MapToDto(o))
        .ToListAsync();
  }

  public async Task<OrderDto?> GetOrderDetailsAsync(int id, string? userId = null)
  {
    var query = context.Orders
        .Include(o => o.User)
        .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
        .AsQueryable();

    if (!string.IsNullOrEmpty(userId))
      query = query.Where(o => o.UserId == userId);

    var order = await query.FirstOrDefaultAsync(o => o.Id == id);
    return order != null ? MapToDto(order) : null;
  }

  public async Task<bool> UpdateOrderStatusAsync(int id, string status, string? paymentStatus = null)
  {
    var order = await context.Orders.FindAsync(id);
    if (order == null) return false;

    if (Enum.TryParse<OrderStatus>(status, out var parsedStatus))
      order.OrderStatus = parsedStatus;

    if (!string.IsNullOrEmpty(paymentStatus) && Enum.TryParse<PaymentStatus>(paymentStatus, out var parsedPayment))
      order.PaymentStatus = parsedPayment;

    return await context.SaveChangesAsync() > 0;
  }

  public async Task<bool> DeleteOrderAsync(int id, string userId)
  {
    var order = await context.Orders
        .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

    if (order == null) return false;

    context.Orders.Remove(order);
    return await context.SaveChangesAsync() > 0;
  }

  private static OrderDto MapToDto(Order o) => new OrderDto
  {
    Id = o.Id,
    OrderDate = o.OrderDate,
    TotalPrice = o.TotalPrice,
    OrderStatus = o.OrderStatus.ToString(),
    PaymentStatus = o.PaymentStatus.ToString(),
    ShippingStreet = o.ShippingStreet,
    ShippingCity = o.ShippingCity,
    ShippingZipCode = o.ShippingZipCode,
    CustomerName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : null,
    CustomerEmail = o.User?.Email,
    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
    {
      ProductId = oi.ProductId,
      ProductName = oi.Product?.Name ?? "Borttagen produkt",
      ProductImageUrl = oi.Product?.ImageUrl ?? "",
      Quantity = oi.Quantity,
      UnitPrice = oi.PriceAtPurchase
    }).ToList()
  };
}