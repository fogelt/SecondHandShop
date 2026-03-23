using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Server.Interfaces;

public interface ICartRepository
{
  Task<IEnumerable<CartItemDto>> GetUserCartAsync(string userId);
  Task<bool> AddToCartAsync(string userId, int productId);
  Task<bool> RemoveFromCartAsync(string userId, int productId);
  Task<bool> ClearCartAsync(string userId);
}