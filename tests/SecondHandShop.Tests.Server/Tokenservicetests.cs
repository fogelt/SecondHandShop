using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SecondHandShop.Server.Data;
using SecondHandShop.Server.Models;
using SecondHandShop.Server.Services;
using SecondHandShop.Shared.Models;
using Xunit;

namespace SecondHandShop.Tests.Services;

public class TokenServiceTests
{
    private readonly TokenService _sut;
    private readonly ApplicationDbContext _context;

    public TokenServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "EnSuperhemligTestnyckelSomMåsteVaraMinst64TeckenLångHär123456!" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" }
            })
            .Build();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sut = new TokenService(config, _context);
    }

    [Fact]
    public void CreateToken_ReturnsValidToken()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            Email = "test@test.com",
            UserName = "test",
            FirstName = "Test",
            LastName = "Testsson"
        };

        var token = _sut.CreateToken(user, new List<string> { "User" });

        Assert.False(string.IsNullOrEmpty(token));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("TestIssuer", jwt.Issuer);
    }

    [Fact]
    public async Task CreateRefreshToken_SavesToDatabase()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            Email = "test@test.com",
            UserName = "test",
            FirstName = "Test",
            LastName = "Testsson"
        };

        var result = await _sut.CreateRefreshTokenAsync(user);

        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.Equal(1, await _context.Set<RefreshToken>().CountAsync());
    }
}