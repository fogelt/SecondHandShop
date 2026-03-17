using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SecondHandShop.Server.Data;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Models;
using SecondHandShop.Shared.Models;

namespace SecondHandShop.Server.Services;

public class TokenService : ITokenService
{
  private readonly IConfiguration _config;
  private readonly ApplicationDbContext _context;
  private readonly SymmetricSecurityKey _key;

  public TokenService(IConfiguration config, ApplicationDbContext context)
  {
    _config = config;
    _context = context;
    _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
  }

  public string CreateToken(ApplicationUser user, IList<string> roles)
  {
    var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.NameId, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new("firstname", user.FirstName),
            new("lastname", user.LastName)
        };

    claims.AddRange(roles.Select(role => new Claim("role", role)));

    var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddMinutes(15),
      SigningCredentials = creds,
      Issuer = _config["Jwt:Issuer"],
      Audience = _config["Jwt:Audience"]
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return tokenHandler.WriteToken(token);
  }

  public async Task<RefreshToken> CreateRefreshTokenAsync(ApplicationUser user)
  {
    var refreshToken = new RefreshToken
    {
      Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      CreatedAt = DateTime.UtcNow,
      UserId = user.Id,
      User = user
    };

    _context.Set<RefreshToken>().Add(refreshToken);
    await _context.SaveChangesAsync();

    return refreshToken;
  }
}