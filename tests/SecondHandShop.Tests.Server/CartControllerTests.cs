using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SecondHandShop.Server.Controllers;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Shared.DTOs;
using System.Security.Claims;
using Xunit;

namespace SecondHandShop.Tests.Server.Controllers;

public class CartControllerTests
{
    private readonly Mock<ICartRepository> _mockRepo;
    private readonly CartController _controller;

    public CartControllerTests()
    {
        _mockRepo = new Mock<ICartRepository>();
        _controller = new CartController(_mockRepo.Object);
        SetUser("user-1");
    }

    private void SetUser(string userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };
    }

    [Fact]
    public async Task GetCart_ReturnsOk()
    {
        var items = new List<CartItemDto> { new() { ProductId = 1, Quantity = 2 } };
        _mockRepo.Setup(r => r.GetUserCartAsync("user-1")).ReturnsAsync(items);

        var result = await _controller.GetCart();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Add_Success_ReturnsOk()
    {
        _mockRepo.Setup(r => r.AddToCartAsync("user-1", 1)).ReturnsAsync(true);

        var result = await _controller.Add(1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Add_Failure_ReturnsBadRequest()
    {
        _mockRepo.Setup(r => r.AddToCartAsync("user-1", 1)).ReturnsAsync(false);

        var result = await _controller.Add(1);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Remove_Success_ReturnsOk()
    {
        _mockRepo.Setup(r => r.RemoveFromCartAsync("user-1", 1)).ReturnsAsync(true);

        var result = await _controller.Remove(1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Remove_NotFound_Returns404()
    {
        _mockRepo.Setup(r => r.RemoveFromCartAsync("user-1", 99)).ReturnsAsync(false);

        var result = await _controller.Remove(99);

        Assert.IsType<NotFoundResult>(result);
    }
}