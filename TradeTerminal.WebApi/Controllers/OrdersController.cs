using Microsoft.AspNetCore.Mvc;
using TradeTerminal.Application.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TradeTerminal.WebApi.Controllers;

/// <summary>
/// Контроллер для работы с заказами
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController(OrderService service) : ControllerBase
{
    private readonly OrderService _service = service;

    /// <summary>
    /// Получить все заказы
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _service.GetAllOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Получить заказ по идентификатору
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _service.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound(new { message = "Заказ не найден" });
        return Ok(order);
    }

    /// <summary>
    /// Получить заказ по номеру
    /// </summary>
    [HttpGet("number/{orderNumber}")]
    public async Task<IActionResult> GetByNumber(int orderNumber)
    {
        var order = await _service.GetOrderByNumberAsync(orderNumber);
        if (order == null)
            return NotFound(new { message = "Заказ не найден" });
        return Ok(order);
    }

    /// <summary>
    /// Получить заказы пользователя
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var orders = await _service.GetOrdersByUserAsync(userId);
        return Ok(orders);
    }

    /// <summary>
    /// Получить заказы по статусу
    /// </summary>
    [HttpGet("status/{statusId}")]
    public async Task<IActionResult> GetByStatus(int statusId)
    {
        var orders = await _service.GetOrdersByStatusAsync(statusId);
        return Ok(orders);
    }

    /// <summary>
    /// Получить заказы за период
    /// </summary>
    [HttpGet("period")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var orders = await _service.GetOrdersByDateRangeAsync(startDate, endDate);
        return Ok(orders);
    }

    /// <summary>
    /// Создать новый заказ
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var random = new Random();
        var pickupCode = random.Next(100, 999).ToString();

        var orderId = await _service.CreateOrderAsync(request.UserId, pickupCode);
        var order = await _service.GetOrderByIdAsync(orderId);

        return CreatedAtAction(nameof(GetById), new { id = orderId }, order);
    }

    /// <summary>
    /// Добавить товар в заказ
    /// </summary>
    [HttpPost("{orderId}/items")]
    public async Task<IActionResult> AddItem(int orderId, [FromBody] AddItemRequest request)
    {
        if (!await _service.OrderExistsAsync(orderId))
            return NotFound(new { message = "Заказ не найден" });

        try
        {
            await _service.AddItemToOrderAsync(orderId, request.ProductId, request.Quantity);
            var order = await _service.GetOrderByIdAsync(orderId);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Удалить товар из заказа
    /// </summary>
    [HttpDelete("{orderId}/items/{productId}")]
    public async Task<IActionResult> RemoveItem(int orderId, int productId)
    {
        if (!await _service.OrderExistsAsync(orderId))
            return NotFound(new { message = "Заказ не найден" });

        await _service.RemoveItemFromOrderAsync(orderId, productId);
        var order = await _service.GetOrderByIdAsync(orderId);
        return Ok(order);
    }

    /// <summary>
    /// Обновить статус заказа
    /// </summary>
    [HttpPut("{orderId}/status")]
    public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] UpdateStatusRequest request)
    {
        if (!await _service.OrderExistsAsync(orderId))
            return NotFound(new { message = "Заказ не найден" });

        await _service.UpdateOrderStatusAsync(orderId, request.StatusId);
        var order = await _service.GetOrderByIdAsync(orderId);
        return Ok(order);
    }

    /// <summary>
    /// Обновить дату доставки
    /// </summary>
    [HttpPut("{orderId}/delivery")]
    public async Task<IActionResult> UpdateDeliveryDate(int orderId, [FromBody] UpdateDeliveryRequest request)
    {
        if (!await _service.OrderExistsAsync(orderId))
            return NotFound(new { message = "Заказ не найден" });

        await _service.UpdateDeliveryDateAsync(orderId, request.DeliveryDate);
        var order = await _service.GetOrderByIdAsync(orderId);
        return Ok(order);
    }

    /// <summary>
    /// Получить товары в заказе
    /// </summary>
    [HttpGet("{orderId}/items")]
    public async Task<IActionResult> GetItems(int orderId)
    {
        if (!await _service.OrderExistsAsync(orderId))
            return NotFound(new { message = "Заказ не найден" });

        var items = await _service.GetOrderItemsAsync(orderId);
        return Ok(items);
    }

    /// <summary>
    /// Получить общую сумму заказа
    /// </summary>
    [HttpGet("{orderId}/total")]
    public async Task<IActionResult> GetTotal(int orderId)
    {
        if (!await _service.OrderExistsAsync(orderId))
            return NotFound(new { message = "Заказ не найден" });

        var total = await _service.CalculateOrderTotalAsync(orderId);
        return Ok(new { total = total });
    }

    /// <summary>
    /// Получить количество заказов
    /// </summary>
    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        var count = await _service.GetOrdersCountAsync();
        return Ok(new { total = count });
    }

    /// <summary>
    /// Получить количество заказов по статусу
    /// </summary>
    [HttpGet("count/status/{statusId}")]
    public async Task<IActionResult> GetCountByStatus(int statusId)
    {
        var count = await _service.GetOrdersCountByStatusAsync(statusId);
        return Ok(new { total = count });
    }

    /// <summary>
    /// Удалить заказ
    /// </summary>
    [HttpDelete("{orderId}")]
    public async Task<IActionResult> Delete(int orderId)
    {
        if (!await _service.OrderExistsAsync(orderId))
            return NotFound(new { message = "Заказ не найден" });

        await _service.DeleteOrderAsync(orderId);
        return Ok(new { message = "Заказ удален" });
    }
}

/// <summary>
/// Запрос на создание заказа
/// </summary>
public class CreateOrderRequest
{
    public int? UserId { get; set; }
}

/// <summary>
/// Запрос на добавление товара в заказ
/// </summary>
public class AddItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

/// <summary>
/// Запрос на обновление статуса
/// </summary>
public class UpdateStatusRequest
{
    public int StatusId { get; set; }
}

/// <summary>
/// Запрос на обновление даты доставки
/// </summary>
public class UpdateDeliveryRequest
{
    public DateTime DeliveryDate { get; set; }
}
