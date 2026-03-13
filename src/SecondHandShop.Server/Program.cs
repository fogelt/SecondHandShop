using Microsoft.EntityFrameworkCore;
using SecondHandShop.Server.Extensions;
using SecondHandShop.Server.Data;
using SecondHandShop.Server.Interfaces;
using SecondHandShop.Server.Services;
using SecondHandShop.Server.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    // Våran seeder från DbSeeder
    await DbSeeder.SeedIdentityAsync(app.Services);

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();