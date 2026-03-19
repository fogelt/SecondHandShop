using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.LocalStorage;

namespace SecondHandShop.Client.Auth;

public class AuthUtility(HttpClient http, ILocalStorageService localStorage)
{
  public async Task EnsureHeader()
  {
    var token = await localStorage.GetItemAsync<string>("accessToken");
    if (!string.IsNullOrEmpty(token))
    {
      http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
  }

  public string ParseIdentityErrors(string rawContent)
  {
    try
    {
      using var doc = JsonDocument.Parse(rawContent);
      if (doc.RootElement.ValueKind == JsonValueKind.Array)
      {
        return string.Join(" ", doc.RootElement.EnumerateArray()
            .Select(e => e.GetProperty("description").GetString())
            .Where(d => !string.IsNullOrEmpty(d)));
      }
    }
    catch { }
    return rawContent;
  }
}