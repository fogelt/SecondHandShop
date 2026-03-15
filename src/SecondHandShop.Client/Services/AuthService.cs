using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SecondHandShop.Client.Auth;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Client.Services;

public class AuthService(HttpClient http, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
{
  public async Task<bool> Login(LoginUserDto loginDto)
  {
    var response = await http.PostAsJsonAsync("api/auth/login", loginDto);

    if (!response.IsSuccessStatusCode) return false;

    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    if (result == null) return false;

    await localStorage.SetItemAsync("accessToken", result.AccessToken);
    await localStorage.SetItemAsync("refreshToken", result.RefreshToken);

    ((CustomAuthStateProvider)authStateProvider).NotifyUserLogin(result.AccessToken);
    return true;
  }

  public async Task Logout()
  {
    await localStorage.RemoveItemAsync("accessToken");
    await localStorage.RemoveItemAsync("refreshToken");
    ((CustomAuthStateProvider)authStateProvider).NotifyUserLogout();
  }
}