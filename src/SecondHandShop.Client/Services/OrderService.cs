using System.Text.Json;
using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Client.Services;

public class OrderService(HttpClient http, CartService cartService)
{
  public async Task<OrderDto?> ConfirmPaymentAsync(string sessionId)
  {
    var response = await http.GetAsync($"api/order/confirm-payment/{sessionId}");

    if (response.IsSuccessStatusCode)
    {
      var order = await response.Content.ReadFromJsonAsync<OrderDto>();
      await cartService.ClearCartAsync();
      return order;
    }
    return null;
  }

  public async Task<List<OrderDto>> GetMyOrdersAsync()
  {
    return await http.GetFromJsonAsync<List<OrderDto>>("api/order/my-orders") ?? [];
  }

  public async Task<OrderDto?> GetOrderDetailsAsync(int id)
  {
    return await http.GetFromJsonAsync<OrderDto>($"api/order/{id}");
  }

  public async Task<string?> GetStripeCheckoutUrl(OrderDto orderDto)
  {
    var response = await http.PostAsJsonAsync("api/payments/create-checkout-session", orderDto);
    if (response.IsSuccessStatusCode)
    {
      var result = await response.Content.ReadFromJsonAsync<JsonElement>();
      return result.GetProperty("url").GetString();
    }
    return null;
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