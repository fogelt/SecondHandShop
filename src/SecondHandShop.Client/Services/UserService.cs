using SecondHandShop.Client.Auth;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Client.Services;

public class UserService(HttpClient http, AuthUtility authUtil)
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
    var response = await http.PostAsJsonAsync("api/auth/update-role", dto);
    return response.IsSuccessStatusCode;
  }

  public async Task<bool> DeleteUser(string userId)
  {
    await authUtil.EnsureHeader();
    var response = await http.DeleteAsync($"api/auth/delete/{userId}");
    return response.IsSuccessStatusCode;
  }
}