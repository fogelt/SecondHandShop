using Microsoft.AspNetCore.Mvc;
using Moq;
using SecondHandShop.Server.Controllers;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Shared.DTOs;
using Xunit;

namespace SecondHandShop.Tests.Server.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthRepository> _mockRepo;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockRepo = new Mock<IAuthRepository>();
        _controller = new AuthController(_mockRepo.Object);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var dto = new LoginRequestDto { Email = "test@test.se", Password = "fel" };
        _mockRepo.Setup(r => r.LoginAsync(dto)).ReturnsAsync((AuthResponseDto?)null);

        var result = await _controller.Login(dto);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        var dto = new LoginRequestDto { Email = "test@test.se", Password = "Pass123!" };
        var response = new AuthResponseDto("token", "refresh", DateTime.Now, "test@test.se", "Test", "Test", new List<string> { "User" });
        _mockRepo.Setup(r => r.LoginAsync(dto)).ReturnsAsync(response);

        var result = await _controller.Login(dto);

        Assert.IsType<OkObjectResult>(result.Result);
    }
}