using SecondHandShop.Shared.DTOs;
using SecondHandShop.Server.Models;

namespace SecondHandShop.Server.Interfaces;

public interface IProductsRepository
{
    public Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    public Task<ProductDto?> GetProductByIdAsync(int id);
    public Task<Product> CreateProductAsync(Product product);
    public Task<Product> UpdateProductAsync(int id, Product product);
    public Task<bool> DeleteProductAsync(int id);
    Task<List<Product>> GetProductsByListAsync(List<int> ids);
    Task MarkProductsAsSoldAsync(IEnumerable<int> ids);
}