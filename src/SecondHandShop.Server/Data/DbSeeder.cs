using SecondHandShop.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace SecondHandShop.Server.Data;

public static class DbSeeder
{
  public static async Task SeedIdentityAsync(IServiceProvider stringProvider)
  {
    using var scope = stringProvider.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
      if (!await roleManager.RoleExistsAsync(role))
        await roleManager.CreateAsync(new IdentityRole(role));
    }

    await CreateUserWithRole(userManager, "admin@secondhand.se", "Admin@12345", "Admin", "Boss");
    await CreateUserWithRole(userManager, "user@secondhand.se", "User@12345", "Standard", "User");
  }

  private static async Task CreateUserWithRole(UserManager<ApplicationUser> um, string email, string password, string first, string last, string role = "User")
  {
    if (await um.FindByEmailAsync(email) == null)
    {
      var user = new ApplicationUser
      {
        UserName = email,
        Email = email,
        EmailConfirmed = true,
        FirstName = first,
        LastName = last
      };
      var result = await um.CreateAsync(user, password);
      if (result.Succeeded) await um.AddToRoleAsync(user, role);
    }
  }
}