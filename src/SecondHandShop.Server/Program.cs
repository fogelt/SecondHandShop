using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecondHandShop.Shared.Data;
using SecondHandShop.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//TBD: Flytta detta för renhetens skull
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//TBD: Flytta detta för renhetens skull
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (await userManager.FindByEmailAsync("test@authdemo.se") == null)
    {
        var testUser = new ApplicationUser
        {
            UserName = "test@authdemo.se",
            Email = "test@authdemo.se",
            FirstName = "Test",
            LastName = "Användare",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(testUser, "Test@12345");

        if (result.Succeeded)
            Console.WriteLine("Testanvändare skapad: test@authdemo.se / Test@12345");
        else
            foreach (var error in result.Errors)
                Console.WriteLine($"Fel: {error.Description}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();