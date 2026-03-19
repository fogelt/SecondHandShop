using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using SecondHandShop.Client.Auth;
using SecondHandShop.Client.Components;
using SecondHandShop.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Razor Components with Interactive Server Mode
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();

// 2. Configure Authentication with a Default Scheme
// This prevents the "No DefaultChallengeScheme found" error.
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        // Where to send the user if they try to access [Authorize] pages without being logged in
        options.LoginPath = "/login";
    });

// 3. Authorization and State Management
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// 4. Custom Auth Provider Setup
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddScoped<AuthService>();

// 5. HttpClient Configuration
builder.Services.AddScoped(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["ApiBaseUrl"];
    if (string.IsNullOrEmpty(baseUrl))
    {
        baseUrl = "http://localhost:5141";
    }
    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});

var app = builder.Build();

// 6. Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// 7. IMPORTANT: Middleware Order
// Authentication MUST come before Authorization
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();