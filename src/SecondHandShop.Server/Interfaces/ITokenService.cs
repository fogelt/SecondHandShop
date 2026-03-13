using SecondHandShop.Shared.Models;
using SecondHandShop.Server.Models;

namespace SecondHandShop.Server.Interfaces;

public interface ITokenService
{
  string CreateToken(ApplicationUser user, IList<string> roles);
  Task<RefreshToken> CreateRefreshTokenAsync(ApplicationUser user);
}