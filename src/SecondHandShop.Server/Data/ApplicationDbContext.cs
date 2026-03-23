using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecondHandShop.Shared.Models;
using SecondHandShop.Server.Models;

namespace SecondHandShop.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options) { }

  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<RefreshToken>()
        .HasOne(rt => rt.User)
        .WithMany()
        .HasForeignKey(rt => rt.UserId)
        .OnDelete(DeleteBehavior.Cascade);
  }

public DbSet<SecondHandShop.Server.Models.Product> Product { get; set; } = default!;
}