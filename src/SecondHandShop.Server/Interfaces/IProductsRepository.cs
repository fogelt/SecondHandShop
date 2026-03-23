using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Server.Interfaces;

public interface IProductsRepository
{
  public Task<IEnumerable<ProductDto>> GetAllProductsAsync();
}