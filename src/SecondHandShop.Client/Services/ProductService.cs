using System.Net.Http.Json;
using SecondHandShop.Client.Auth;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Client.Services;

public class ProductService(HttpClient http, AuthUtility authUtil)
{
  private readonly HttpClient _http = http;
  private readonly AuthUtility _authUtil = authUtil;

  public async Task<List<ProductDto>> GetAllProductsAsync()
  {
    return await _http.GetFromJsonAsync<List<ProductDto>>("api/products") ?? [];
  }

  public async Task<ProductDto?> GetProductByIdAsync(int id)
  {
    try
    {
      return await _http.GetFromJsonAsync<ProductDto>($"api/products/{id}");
    }
    catch
    {
      return null;
    }
  }

  // Admin Only: Requires Auth Header
  public async Task<bool> CreateProductAsync(ProductDto product)
  {
    await _authUtil.EnsureHeader();
    var response = await _http.PostAsJsonAsync("api/products", product);
    return response.IsSuccessStatusCode;
  }

  // Admin Only: Requires Auth Header
  public async Task<bool> UpdateProductAsync(int id, ProductDto product)
  {
    await _authUtil.EnsureHeader();
    var response = await _http.PutAsJsonAsync($"api/products/{id}", product);
    return response.IsSuccessStatusCode;
  }

  // Admin Only: Requires Auth Header
  public async Task<bool> DeleteProductAsync(int id)
  {
    await _authUtil.EnsureHeader();
    var response = await _http.DeleteAsync($"api/products/{id}");
    return response.IsSuccessStatusCode;
  }
}