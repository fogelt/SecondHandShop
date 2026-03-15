using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace SecondHandShop.Client.Auth;

public class CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http) : AuthenticationStateProvider
{
  public override async Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    try
    {
      var token = await localStorage.GetItemAsync<string>("accessToken");

      if (string.IsNullOrWhiteSpace(token))
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

      http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
    }
    catch (Exception)
    {
      return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
  }

  public void NotifyUserLogin(string token)
  {
    var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
    var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
    NotifyAuthenticationStateChanged(authState);
  }

  public void NotifyUserLogout()
  {
    var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
    var authState = Task.FromResult(new AuthenticationState(anonymousUser));
    NotifyAuthenticationStateChanged(authState);
  }

  private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
  {
    var payload = jwt.Split('.')[1];
    var jsonBytes = ParseBase64WithoutPadding(payload);
    var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
    return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
  }

  private byte[] ParseBase64WithoutPadding(string base64)
  {
    switch (base64.Length % 4)
    {
      case 2: base64 += "=="; break;
      case 3: base64 += "="; break;
    }
    return Convert.FromBase64String(base64);
  }
}