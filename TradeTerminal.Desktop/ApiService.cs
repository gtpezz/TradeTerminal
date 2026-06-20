using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace TradeTerminal.Desktop;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public ApiService()
    {
        _baseUrl = Properties.Settings.Default.ApiBaseUrl;
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    #region Products

    public async Task<JsonElement> GetProductsAsync()
    {
        var response = await _http.GetAsync($"{_baseUrl}/api/products");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    public async Task<JsonElement> GetProductsWithFilterAsync(int? manufacturerId = null, int? categoryId = null,
        decimal? minPrice = null, decimal? maxPrice = null)
    {
        var url = $"{_baseUrl}/api/products/filter";
        var parameters = new List<string>();
        if (manufacturerId.HasValue) parameters.Add($"manufacturerId={manufacturerId}");
        if (categoryId.HasValue) parameters.Add($"categoryId={categoryId}");
        if (minPrice.HasValue) parameters.Add($"minPrice={minPrice}");
        if (maxPrice.HasValue) parameters.Add($"maxPrice={maxPrice}");
        if (parameters.Any()) url += "?" + string.Join("&", parameters);

        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    public async Task<JsonElement> SearchProductsAsync(string term)
    {
        var response = await _http.GetAsync($"{_baseUrl}/api/products/search?term={Uri.EscapeDataString(term)}");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    #endregion

    #region Orders

    public async Task<JsonElement> GetOrdersAsync()
    {
        var response = await _http.GetAsync($"{_baseUrl}/api/orders");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    public async Task<JsonElement> GetOrderByIdAsync(int id)
    {
        var response = await _http.GetAsync($"{_baseUrl}/api/orders/{id}");
        if (!response.IsSuccessStatusCode) 
            return default;
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    public async Task<JsonElement> GetOrderByNumberAsync(int orderNumber)
    {
        var response = await _http.GetAsync($"{_baseUrl}/api/orders/number/{orderNumber}");
        if (!response.IsSuccessStatusCode) 
            return default;
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    public async Task<int> CreateOrderAsync(int? userId)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync($"{_baseUrl}/api/orders", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonElement>(json).GetProperty("id").GetInt32();
    }

    public async Task<JsonElement> AddItemToOrderAsync(int orderId, int productId, int quantity = 1)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { productId, quantity }), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync($"{_baseUrl}/api/orders/{orderId}/items", content);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    public async Task<JsonElement> RemoveItemFromOrderAsync(int orderId, int productId)
    {
        var response = await _http.DeleteAsync($"{_baseUrl}/api/orders/{orderId}/items/{productId}");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    public async Task<JsonElement> UpdateOrderStatusAsync(int orderId, int statusId)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { statusId }), Encoding.UTF8, "application/json");
        var response = await _http.PutAsync($"{_baseUrl}/api/orders/{orderId}/status", content);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    public async Task<JsonElement> UpdateDeliveryDateAsync(int orderId, DateTime date)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { deliveryDate = date }), Encoding.UTF8, "application/json");
        var response = await _http.PutAsync($"{_baseUrl}/api/orders/{orderId}/delivery", content);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        await _http.DeleteAsync($"{_baseUrl}/api/orders/{orderId}");
    }

    #endregion

    #region Auth

    public async Task<JsonElement> AuthenticateAsync(string login, string password)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { login, password }), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync($"{_baseUrl}/api/auth/login", content);
        if (!response.IsSuccessStatusCode) 
            return default;
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    #endregion

    #region References

    public async Task<JsonElement> GetManufacturersAsync()
    {
        var response = await _http.GetAsync($"{_baseUrl}/api/manufacturers");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    public async Task<JsonElement> GetCategoriesAsync()
    {
        var response = await _http.GetAsync($"{_baseUrl}/api/categories");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }

    public async Task<JsonElement> GetOrderStatusesAsync()
    {
        var response = await _http.GetAsync($"{_baseUrl}/api/orderstatuses");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
    }
}

    #endregion
