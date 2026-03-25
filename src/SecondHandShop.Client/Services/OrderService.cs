using SecondHandShop.Shared.DTOs;

namespace SecondHandShop.Client.Services;

public class OrderService(HttpClient http)
{
  public async Task<OrderDto?> CreateOrderAsync(OrderDto orderDto)
  {
    var response = await http.PostAsJsonAsync("api/order/create", orderDto);

    if (response.IsSuccessStatusCode)
    {
      return await response.Content.ReadFromJsonAsync<OrderDto>();
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
}