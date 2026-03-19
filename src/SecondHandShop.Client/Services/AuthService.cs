using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SecondHandShop.Client.Auth;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Client.Services;

public class AuthService(HttpClient http, ILocalStorageService localStorage,
                         AuthenticationStateProvider authStateProvider, AuthUtility authUtil)
{
  public async Task<string?> Login(LoginRequestDto request)
  {
    var response = await http.PostAsJsonAsync("api/auth/login", request);

    if (!response.IsSuccessStatusCode)
      return authUtil.ParseIdentityErrors(await response.Content.ReadAsStringAsync());

    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    if (result == null) return "Inloggning misslyckades.";

    await localStorage.SetItemAsync("accessToken", result.AccessToken);
    await localStorage.SetItemAsync("refreshToken", result.RefreshToken);

    await authUtil.EnsureHeader();

    ((CustomAuthStateProvider)authStateProvider).NotifyUserLogin(result.AccessToken);
    return null;
  }

  public async Task<string?> Register(RegisterRequestDto request)
  {
    var response = await http.PostAsJsonAsync("api/auth/register", request);

    if (!response.IsSuccessStatusCode)
    {
      var rawContent = await response.Content.ReadAsStringAsync();
      return authUtil.ParseIdentityErrors(rawContent);
    }
    return null;
  }

  public async Task Logout()
  {
    await localStorage.RemoveItemAsync("accessToken");
    await localStorage.RemoveItemAsync("refreshToken");

    http.DefaultRequestHeaders.Authorization = null;
    ((CustomAuthStateProvider)authStateProvider).NotifyUserLogout();
  }
}