using SecondHandShop.Shared.Models;
using Microsoft.AspNetCore.Identity;
using SecondHandShop.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using SecondHandShop.Server.Models;

namespace SecondHandShop.Server.Data;

public static class DbSeeder
{
  public static async Task SeedDataAsync(IServiceProvider serviceProvider)
  {
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedIdentityAsync(userManager, roleManager);

    await SeedProductsAsync(context);
  }

  private static async Task SeedIdentityAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
  {
    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
      if (!await roleManager.RoleExistsAsync(role))
        await roleManager.CreateAsync(new IdentityRole(role));
    }

    await CreateUserWithRole(userManager, "admin@secondhand.se", "Admin@12345", "Admin", "Boss", Roles.Admin);
    await CreateUserWithRole(userManager, "user@secondhand.se", "User@12345", "Standard", "User", Roles.User);
  }

  private static async Task SeedProductsAsync(ApplicationDbContext context)
  {
    if (await context.Products.AnyAsync()) return;

    var products = new List<Product>
        {
            new() {
                Name = "Vintage Camera",
                Description = "A well-preserved film camera from the 70s.",
                Price = 1200.00m,
                ImageUrl = "https://images.unsplash.com/photo-1516035069371-29a1b244cc32"
            },
            new() {
                Name = "Leather Jacket",
                Description = "Classic black leather jacket, size Large. Minor wear.",
                Price = 850.50m,
                ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16eac5"
            },
            new() {
                Name = "Mechanical Keyboard",
                Description = "RGB backlit with blue switches. Great for typing.",
                Price = 450.00m,
                ImageUrl = "https://images.unsplash.com/photo-1511467687858-23d96c32e4ae"
            }
        };

    await context.Products.AddRangeAsync(products);
    await context.SaveChangesAsync();
  }

  private static async Task CreateUserWithRole(UserManager<ApplicationUser> um, string email, string password, string first, string last, Roles role = Roles.User)
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
      if (result.Succeeded) await um.AddToRoleAsync(user, role.ToString());
    }
  }
}