using Microsoft.EntityFrameworkCore;
using SecondHandShop.Server.Data;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Models;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Server.Repositories;

public class CartRepository(ApplicationDbContext context) : ICartRepository
{
  private readonly ApplicationDbContext _context = context;

  public async Task<IEnumerable<CartItemDto>> GetUserCartAsync(string userId)
  {
    return await _context.CartItems
        .Where(ci => ci.UserId == userId)
        .Include(ci => ci.Product)
        .Select(ci => new CartItemDto
        {
          ProductId = ci.ProductId,
          ProductName = ci.Product.Name,
          Price = ci.Product.Price,
          ImageUrl = ci.Product.ImageUrl,
          Quantity = ci.Quantity
        }).ToListAsync();
  }

  public async Task<bool> AddToCartAsync(string userId, int productId)
  {
    var existingItem = await _context.CartItems
        .AnyAsync(ci => ci.UserId == userId && ci.ProductId == productId);

    if (existingItem)
    {
      return true;
    }

    var product = await _context.Products
        .FirstOrDefaultAsync(p => p.Id == productId && !p.IsSold);

    if (product == null)
    {
      return false;
    }

    _context.CartItems.Add(new CartItem
    {
      UserId = userId,
      ProductId = productId,
      Quantity = 1
    });

    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> RemoveFromCartAsync(string userId, int productId)
  {
    var item = await _context.CartItems
        .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

    if (item == null) return false;

    _context.CartItems.Remove(item);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> ClearCartAsync(string userId)
  {
    var items = await _context.CartItems.Where(ci => ci.UserId == userId).ToListAsync();
    _context.CartItems.RemoveRange(items);
    return await _context.SaveChangesAsync() > 0;
  }
}