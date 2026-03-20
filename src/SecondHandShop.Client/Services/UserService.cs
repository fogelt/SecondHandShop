using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SecondHandShop.Client.Auth;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Client.Services;

public class UserService(HttpClient http, AuthUtility authUtil, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
{
  public async Task<List<UserDto>> GetAllUsers()
  {
    await authUtil.EnsureHeader();
    var response = await http.GetAsync("api/auth/get-all");
    return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<UserDto>>() ?? [] : [];
  }

  public async Task<bool> UpdateUserRole(UpdateRoleDto dto)
  {
    await authUtil.EnsureHeader();
    var response = await http.PutAsJsonAsync("api/auth/update-role", dto);
    return response.IsSuccessStatusCode;
  }

  public async Task<string?> UpdateUserAsync(UpdateUserDto dto)
  {
    await authUtil.EnsureHeader();
    var response = await http.PutAsJsonAsync($"api/auth/update-user/{dto.Id}", dto);

    if (response.IsSuccessStatusCode)
    {
      var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
      if (result != null)
      {
        await localStorage.SetItemAsync("accessToken", result.AccessToken);
        await localStorage.SetItemAsync("refreshToken", result.RefreshToken);
        if (authStateProvider is CustomAuthStateProvider customProvider)
        {
          customProvider.NotifyUserLogin(result.AccessToken);
        }
      }
      return null;
    }
    var rawContent = await response.Content.ReadAsStringAsync();
    return authUtil.ParseIdentityErrors(rawContent);
  }

  public async Task<bool> DeleteUser(string userId)
  {
    await authUtil.EnsureHeader();
    var response = await http.DeleteAsync($"api/auth/delete/{userId}");
    return response.IsSuccessStatusCode;
  }
}