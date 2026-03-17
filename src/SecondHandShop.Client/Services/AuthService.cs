using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SecondHandShop.Client.Auth;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Client.Services;

public class AuthService(HttpClient http, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
{
  public async Task<string?> Login(LoginRequestDto request)
  {
    var response = await http.PostAsJsonAsync("api/auth/login", request);

    if (!response.IsSuccessStatusCode)
    {
      var rawContent = await response.Content.ReadAsStringAsync();
      return ParseIdentityErrors(rawContent);
    }

    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    if (result == null) return "Ett fel uppstod vid inloggning.";

    await localStorage.SetItemAsync("accessToken", result.AccessToken);
    await localStorage.SetItemAsync("refreshToken", result.RefreshToken);
    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);

    ((CustomAuthStateProvider)authStateProvider).NotifyUserLogin(result.AccessToken);
    return null;
  }

  public async Task<string?> Register(RegisterRequestDto request)
  {
    var response = await http.PostAsJsonAsync("api/auth/register", request);

    if (!response.IsSuccessStatusCode)
    {
      var rawContent = await response.Content.ReadAsStringAsync();
      return ParseIdentityErrors(rawContent);
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

  private string ParseIdentityErrors(string rawContent)
  {
    try
    {
      using var doc = JsonDocument.Parse(rawContent);
      if (doc.RootElement.ValueKind == JsonValueKind.Array)
      {
        var errors = doc.RootElement.EnumerateArray()
            .Select(e => e.GetProperty("description").GetString())
            .Where(desc => !string.IsNullOrEmpty(desc));

        return string.Join(" ", errors!);
      }
    }
    catch
    {
      // Om parsingen misslycaks, returnerar vi den råa strängen
    }

    return rawContent;
  }
}