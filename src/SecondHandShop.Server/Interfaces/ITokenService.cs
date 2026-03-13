using SecondHandShop.Shared.Models;
using SecondHandShop.Server.Models;

namespace SecondHandShop.Server.Interfaces;

public interface ITokenService
{
  string GenerateAccessToken(ApplicationUser user, IList<string> roles);
  string GenerateRefreshToken();
  Task<RefreshToken> CreateRefreshTokenAsync(ApplicationUser user);
}