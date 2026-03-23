using Microsoft.EntityFrameworkCore;
using SecondHandShop.Server.Data;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Server.Repositories;

public class ProductsRepository(ApplicationDbContext context) : IProductsRepository
{
  private readonly ApplicationDbContext _context = context;

  public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
  {
    return await _context.Products
        .Select(p => new ProductDto
        {
          Id = p.Id,
          Name = p.Name,
          Description = p.Description,
          Price = p.Price,
          ImageUrl = p.ImageUrl
        })
        .ToListAsync();
  }
}