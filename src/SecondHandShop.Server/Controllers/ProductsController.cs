using SecondHandShop.Shared.DTOs;
using SecondHandShop.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SecondHandShop.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductsRepository productRepo) : ControllerBase
{
  private readonly IProductsRepository _productRepo = productRepo;

  [HttpGet]
  public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
  {
    var products = await _productRepo.GetAllProductsAsync();

    if (products == null || !products.Any())
    {
      return NotFound("No products found.");
    }

    return Ok(products);
  }
}