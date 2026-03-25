using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Client.Auth;

public class AuthUtility(HttpClient http, ILocalStorageService localStorage)
{
  private static readonly SemaphoreSlim _refreshSemaphore = new(1, 1);

  public async Task EnsureHeader()
  {
    if (http == null || localStorage == null) return;

    try
    {
      var headers = http.DefaultRequestHeaders;
      if (headers == null) return;

      string? token = await localStorage.GetItemAsync<string>("accessToken");

      if (string.IsNullOrWhiteSpace(token))
      {
        headers.Authorization = null;
        return;
      }

      if (IsTokenExpired(token))
      {
        await _refreshSemaphore.WaitAsync();
        try
        {
          token = await localStorage.GetItemAsync<string>("accessToken");
          if (!string.IsNullOrEmpty(token) && IsTokenExpired(token))
          {
            token = await TryRefreshToken();
          }
        }
        finally
        {
          _refreshSemaphore.Release();
        }
      }

      if (!string.IsNullOrEmpty(token))
      {
        headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
      }
      else
      {
        headers.Authorization = null;
      }
    }
    catch (NullReferenceException)
    {
      Console.WriteLine("AuthUtility: Circuit transition recovery.");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"AuthUtility Error: {ex.Message}");
    }
  }

  private bool IsTokenExpired(string token)
  {
    if (string.IsNullOrWhiteSpace(token)) return true;

    try
    {
      var handler = new JwtSecurityTokenHandler();
      if (!handler.CanReadToken(token)) return true;

      var jwtToken = handler.ReadJwtToken(token);
      return jwtToken == null || jwtToken.ValidTo < DateTime.UtcNow.AddSeconds(20);
    }
    catch
    {
      return true;
    }
  }

  private async Task<string?> TryRefreshToken()
  {
    try
    {
      var refreshToken = await localStorage.GetItemAsync<string>("refreshToken");
      if (string.IsNullOrEmpty(refreshToken) || http == null) return null;

      var response = await http.PostAsJsonAsync("api/auth/refresh", refreshToken);

      if (response.IsSuccessStatusCode)
      {
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (result != null)
        {
          await localStorage.SetItemAsync("accessToken", result.AccessToken);
          await localStorage.SetItemAsync("refreshToken", result.RefreshToken);
          return result.AccessToken;
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Refresh Failed: {ex.Message}");
    }

    await localStorage.RemoveItemAsync("accessToken");
    await localStorage.RemoveItemAsync("refreshToken");
    return null;
  }

  public string ParseIdentityErrors(string rawContent)
  {
    try
    {
      using var doc = JsonDocument.Parse(rawContent);
      var root = doc.RootElement;
      if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("errors", out var errorsElement))
      {
        var messages = new List<string>();
        foreach (var property in errorsElement.EnumerateObject())
        {
          foreach (var error in property.Value.EnumerateArray())
          {
            messages.Add(error.GetString() ?? "");
          }
        }
        return string.Join(", ", messages.Where(m => !string.IsNullOrEmpty(m)));
      }
      if (root.ValueKind == JsonValueKind.Array)
      {
        return string.Join(", ", root.EnumerateArray()
            .Select(e => e.GetProperty("description").GetString())
            .Where(d => !string.IsNullOrEmpty(d)));
      }
    }
    catch { }

    return rawContent;
  }
}