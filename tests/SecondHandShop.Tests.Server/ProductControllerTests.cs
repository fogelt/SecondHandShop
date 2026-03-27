using Microsoft.AspNetCore.Mvc;
using Moq;
using SecondHandShop.Server.Controllers;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Models;
using SecondHandShop.Shared.DTOs;
using Xunit;

namespace SecondHandShop.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductsRepository> _mockRepo;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockRepo = new Mock<IProductsRepository>();
        _controller = new ProductsController(_mockRepo.Object);
    }

    // ── GetAll ──────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ProductsExist_ReturnsOkWithProducts()
    {
        var products = new List<ProductDto>
        {
            new() { Id = 1, Name = "Jacka", Price = 199 },
            new() { Id = 2, Name = "Byxor", Price = 149 }
        };
        _mockRepo.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(products);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(2, returned.Count());
    }

    [Fact]
    public async Task GetAll_NoProducts_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetAllProductsAsync())
                  .ReturnsAsync(new List<ProductDto>());

        var result = await _controller.GetAll();

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No products found.", notFound.Value);
    }

    // ── GetById ─────────────────────────────────────────────

    [Fact]
    public async Task GetById_ProductExists_ReturnsOk()
    {
        var product = new ProductDto { Id = 1, Name = "Jacka", Price = 199 };
        _mockRepo.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal("Jacka", returned.Name);
    }

    [Fact]
    public async Task GetById_ProductNotFound_Returns404()
    {
        _mockRepo.Setup(r => r.GetProductByIdAsync(99))
                  .ReturnsAsync((ProductDto?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    // ── Create ──────────────────────────────────────────────

    [Fact]
    public async Task Create_ValidProduct_ReturnsCreatedAtAction()
    {
        var product = new Product { Id = 0, Name = "Skor", Price = 299 };
        var created = new Product { Id = 5, Name = "Skor", Price = 299 };
        _mockRepo.Setup(r => r.CreateProductAsync(product)).ReturnsAsync(created);

        var result = await _controller.Create(product);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
        var returned = Assert.IsType<Product>(createdResult.Value);
        Assert.Equal(5, returned.Id);
    }

    // ── Update ──────────────────────────────────────────────

    [Fact]
    public async Task Update_ProductExists_ReturnsNoContent()
    {
        var product = new Product { Id = 1, Name = "Uppdaterad", Price = 399 };
        _mockRepo.Setup(r => r.UpdateProductAsync(1, product))
                  .ReturnsAsync(product);

        var result = await _controller.Update(1, product);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ProductNotFound_Returns404()
    {
        var product = new Product { Id = 99, Name = "Finns ej", Price = 0 };
        _mockRepo.Setup(r => r.UpdateProductAsync(99, product))
                  .ThrowsAsync(new KeyNotFoundException());

        var result = await _controller.Update(99, product);

        Assert.IsType<NotFoundResult>(result);
    }

    // ── Delete ──────────────────────────────────────────────

    [Fact]
    public async Task Delete_ProductExists_ReturnsNoContent()
    {
        _mockRepo.Setup(r => r.DeleteProductAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ProductNotFound_Returns404()
    {
        _mockRepo.Setup(r => r.DeleteProductAsync(99)).ReturnsAsync(false);

        var result = await _controller.Delete(99);

        Assert.IsType<NotFoundResult>(result);
    }
}