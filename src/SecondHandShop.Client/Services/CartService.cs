using SecondHandShop.Client.Auth;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Client.Services;

public class CartService(HttpClient http, AuthUtility authUtil)
{
  private readonly HttpClient _http = http;
  private readonly AuthUtility _authUtil = authUtil;
  public List<CartItemDto> CartItems { get; private set; } = [];

  public async Task<List<CartItemDto>> GetCartAsync()
  {
    await _authUtil.EnsureHeader();
    var response = await _http.GetAsync("api/cart");

    if (response.IsSuccessStatusCode)
    {
      CartItems = await response.Content.ReadFromJsonAsync<List<CartItemDto>>() ?? [];
    }

    return CartItems;
  }

  public async Task<bool> AddToCartAsync(int productId)
  {
    await _authUtil.EnsureHeader();
    var response = await _http.PostAsync($"api/cart/add/{productId}", null);

    if (response.IsSuccessStatusCode)
    {
      await GetCartAsync();
      return true;
    }
    return false;
  }

  public async Task<bool> RemoveFromCartAsync(int productId)
  {
    await _authUtil.EnsureHeader();
    var response = await _http.DeleteAsync($"api/cart/{productId}");

    if (response.IsSuccessStatusCode)
    {
      await GetCartAsync();
      return true;
    }
    return false;
  }

  public async Task ClearCartAsync() => CartItems.Clear();
  public decimal GetTotal() => CartItems.Sum(x => x.Total);
  public int GetTotalItems() => CartItems.Sum(x => x.Quantity);
}