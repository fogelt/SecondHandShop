using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecondHandShop.Shared.Models;
using SecondHandShop.Server.Models;
using Microsoft.Net.Http.Headers;

namespace SecondHandShop.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options) { }

  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
  public DbSet<Product> Products => Set<Product>();
  public DbSet<CartItem> CartItems => Set<CartItem>();
  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<RefreshToken>()
        .HasOne(rt => rt.User)
        .WithMany()
        .HasForeignKey(rt => rt.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Entity<CartItem>(entity =>
        {
          entity.HasOne(ci => ci.User)
              .WithMany(u => u.Cart)
              .HasForeignKey(ci => ci.UserId)
              .OnDelete(DeleteBehavior.Cascade);

          entity.HasOne(ci => ci.Product)
              .WithMany()
              .HasForeignKey(ci => ci.ProductId)
              .OnDelete(DeleteBehavior.Cascade);
        });
  }
}