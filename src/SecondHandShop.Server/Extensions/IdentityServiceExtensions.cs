using Microsoft.AspNetCore.Identity;
using SecondHandShop.Shared.Models;
using SecondHandShop.Server.Data;

namespace SecondHandShop.Server.Extensions;

public static class IdentityServiceExtensions
{
  public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
  {
    services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
      options.Password.RequiredLength = 8;
      options.Password.RequireDigit = true;
      options.Password.RequireUppercase = true;
      options.Password.RequireNonAlphanumeric = true;
      options.Lockout.MaxFailedAccessAttempts = 5;
      options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
      options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    return services;
  }
}