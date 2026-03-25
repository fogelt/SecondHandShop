using System.Net.Http.Json;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Client.Services;

public class OrderService(HttpClient http)
{
  public async Task<OrderDto?> CreateOrderAsync(OrderDto orderDto)
  {
    var response = await http.PostAsJsonAsync("api/order/create", orderDto);
    return response.IsSuccessStatusCode
        ? await response.Content.ReadFromJsonAsync<OrderDto>()
        : null;
  }

  public async Task<List<OrderDto>> GetMyOrdersAsync()
  {
    return await http.GetFromJsonAsync<List<OrderDto>>("api/order/my-orders") ?? [];
  }

  public async Task<OrderDto?> GetOrderDetailsAsync(int id)
  {
    return await http.GetFromJsonAsync<OrderDto>($"api/order/{id}");
  }

  // ADMIN
  public async Task<List<OrderDto>> GetAllOrdersAdminAsync()
  {
    return await http.GetFromJsonAsync<List<OrderDto>>("api/order/admin/all") ?? [];
  }

  // ADMIN
  public async Task<bool> UpdateOrderStatusAsync(int id, string status, string? paymentStatus = null)
  {
    var url = $"api/order/admin/update-status/{id}?status={status}";
    if (!string.IsNullOrEmpty(paymentStatus))
    {
      url += $"&paymentStatus={paymentStatus}";
    }

    var response = await http.PutAsync(url, null);
    return response.IsSuccessStatusCode;
  }
}