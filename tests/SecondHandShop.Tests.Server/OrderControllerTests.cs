using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SecondHandShop.Server.Controllers;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Shared.DTOs;
using SecondHandShop.Server.Services;
using System.Security.Claims;
using Xunit;

namespace SecondHandShop.Tests.Server.Controllers;

public class OrderControllerTests
{
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<IOrderRepository> _mockOrderRepo;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _mockOrderRepo = new Mock<IOrderRepository>();
        _mockOrderService = new Mock<IOrderService>();
        _controller = new OrderController(_mockOrderService.Object, _mockOrderRepo.Object);
        SetUser("user-1");
    }

    private void SetUser(string userId, string role = "User")
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Role, role)
        };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };
    }

    [Fact]
    public async Task GetMyOrders_ReturnsOk()
    {
        _mockOrderRepo.Setup(r => r.GetUserOrdersAsync("user-1"))
                       .ReturnsAsync(new List<OrderDto>());

        var result = await _controller.GetMyOrders();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetDetails_OrderExists_ReturnsOk()
    {
        _mockOrderRepo.Setup(r => r.GetOrderDetailsAsync(1, "user-1"))
                       .ReturnsAsync(new OrderDto());

        var result = await _controller.GetDetails(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetDetails_OrderNotFound_Returns404()
    {
        _mockOrderRepo.Setup(r => r.GetOrderDetailsAsync(99, "user-1"))
                       .ReturnsAsync((OrderDto?)null);

        var result = await _controller.GetDetails(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_Success_ReturnsOk()
    {
        _mockOrderRepo.Setup(r => r.UpdateOrderStatusAsync(1, "Skickad", null))
                       .ReturnsAsync(true);

        var result = await _controller.UpdateStatus(1, "Skickad");

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_Failure_ReturnsBadRequest()
    {
        _mockOrderRepo.Setup(r => r.UpdateOrderStatusAsync(1, "Ogiltig", null))
                       .ReturnsAsync(false);

        var result = await _controller.UpdateStatus(1, "Ogiltig");

        Assert.IsType<BadRequestObjectResult>(result);
    }
}