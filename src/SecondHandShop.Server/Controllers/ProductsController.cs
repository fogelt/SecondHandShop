using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Models;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductsRepository productRepo) : ControllerBase
{
    private readonly IProductsRepository _productRepo = productRepo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productRepo.GetAllProductsAsync();

        if (products == null || !products.Any())
        {
            return NotFound("No products found.");
        }

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productRepo.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        var createdProduct = await _productRepo.CreateProductAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        try
        {
            await _productRepo.UpdateProductAsync(id, product);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productRepo.DeleteProductAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}