using Microsoft.EntityFrameworkCore;
using TradeTerminal.Application.DTOs;
using TradeTerminal.DataAccess;
using TradeTerminal.Domain.Models;

namespace TradeTerminal.Application.Services;


/// <summary>
/// Сервис для работы с заказами
/// </summary>
public class OrderService
{
    private readonly DatabaseContext _context;

    public OrderService(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить все заказы в виде DTO
    /// </summary>
    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserId = o.UserId,
                UserFullName = o.User != null ? o.User.FullName : null,
                OrderDate = o.OrderDate,
                DeliveryDate = o.DeliveryDate,
                StatusId = o.StatusId,
                StatusName = o.Status != null ? o.Status.Name : null,
                PickupCode = o.PickupCode,
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : null,
                    ProductArticle = oi.Product != null ? oi.Product.Article : null,
                    Quantity = oi.Quantity,
                    PriceAtOrder = oi.PriceAtOrder,
                    DiscountAtOrder = oi.DiscountAtOrder
                }).ToList()
            })
            .ToListAsync();
    }

    /// <summary>
    /// Получить заказ по идентификатору в виде DTO
    /// </summary>
    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.Id == id)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserId = o.UserId,
                UserFullName = o.User != null ? o.User.FullName : null,
                OrderDate = o.OrderDate,
                DeliveryDate = o.DeliveryDate,
                StatusId = o.StatusId,
                StatusName = o.Status != null ? o.Status.Name : null,
                PickupCode = o.PickupCode,
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : null,
                    ProductArticle = oi.Product != null ? oi.Product.Article : null,
                    Quantity = oi.Quantity,
                    PriceAtOrder = oi.PriceAtOrder,
                    DiscountAtOrder = oi.DiscountAtOrder
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Получить заказ по номеру в виде DTO
    /// </summary>
    public async Task<OrderDto?> GetOrderByNumberAsync(int orderNumber)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.OrderNumber == orderNumber)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserId = o.UserId,
                UserFullName = o.User != null ? o.User.FullName : null,
                OrderDate = o.OrderDate,
                DeliveryDate = o.DeliveryDate,
                StatusId = o.StatusId,
                StatusName = o.Status != null ? o.Status.Name : null,
                PickupCode = o.PickupCode,
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : null,
                    ProductArticle = oi.Product != null ? oi.Product.Article : null,
                    Quantity = oi.Quantity,
                    PriceAtOrder = oi.PriceAtOrder,
                    DiscountAtOrder = oi.DiscountAtOrder
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Получить заказы пользователя в виде DTO
    /// </summary>
    public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserId = o.UserId,
                UserFullName = o.User != null ? o.User.FullName : null,
                OrderDate = o.OrderDate,
                DeliveryDate = o.DeliveryDate,
                StatusId = o.StatusId,
                StatusName = o.Status != null ? o.Status.Name : null,
                PickupCode = o.PickupCode,
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : null,
                    ProductArticle = oi.Product != null ? oi.Product.Article : null,
                    Quantity = oi.Quantity,
                    PriceAtOrder = oi.PriceAtOrder,
                    DiscountAtOrder = oi.DiscountAtOrder
                }).ToList()
            })
            .ToListAsync();
    }

    /// <summary>
    /// Получить заказы по статусу в виде DTO
    /// </summary>
    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(int statusId)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.StatusId == statusId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserId = o.UserId,
                UserFullName = o.User != null ? o.User.FullName : null,
                OrderDate = o.OrderDate,
                DeliveryDate = o.DeliveryDate,
                StatusId = o.StatusId,
                StatusName = o.Status != null ? o.Status.Name : null,
                PickupCode = o.PickupCode,
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : null,
                    ProductArticle = oi.Product != null ? oi.Product.Article : null,
                    Quantity = oi.Quantity,
                    PriceAtOrder = oi.PriceAtOrder,
                    DiscountAtOrder = oi.DiscountAtOrder
                }).ToList()
            })
            .ToListAsync();
    }

    /// <summary>
    /// Получить заказы за период в виде DTO
    /// </summary>
    public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserId = o.UserId,
                UserFullName = o.User != null ? o.User.FullName : null,
                OrderDate = o.OrderDate,
                DeliveryDate = o.DeliveryDate,
                StatusId = o.StatusId,
                StatusName = o.Status != null ? o.Status.Name : null,
                PickupCode = o.PickupCode,
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : null,
                    ProductArticle = oi.Product != null ? oi.Product.Article : null,
                    Quantity = oi.Quantity,
                    PriceAtOrder = oi.PriceAtOrder,
                    DiscountAtOrder = oi.DiscountAtOrder
                }).ToList()
            })
            .ToListAsync();
    }

    /// <summary>
    /// Создать новый заказ
    /// </summary>
    public async Task<int> CreateOrderAsync(int? userId, string pickupCode)
    {
        var maxOrderNumber = await _context.Orders
            .MaxAsync(o => (int?)o.OrderNumber) ?? 0;

        var order = new Order
        {
            OrderNumber = maxOrderNumber + 1,
            UserId = userId,
            OrderDate = DateTime.Now,
            StatusId = 1, // "Новый"
            PickupCode = pickupCode,
            TotalAmount = 0
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order.Id;
    }

    /// <summary>
    /// Добавить товар в заказ
    /// </summary>
    public async Task AddItemToOrderAsync(int orderId, int productId, int quantity = 1)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            throw new Exception("Товар не найден");

        if (product.StockQuantity < quantity)
            throw new Exception("Недостаточно товара на складе");

        var existingItem = await _context.OrderItems
            .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var orderItem = new OrderItem
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
                PriceAtOrder = product.Price,
                DiscountAtOrder = product.Discount
            };
            _context.OrderItems.Add(orderItem);
        }

        await _context.SaveChangesAsync();

        await UpdateOrderTotalAsync(orderId);
    }

    /// <summary>
    /// Обновить статус заказа
    /// </summary>
    public async Task UpdateOrderStatusAsync(int orderId, int statusId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.StatusId = statusId;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Обновить дату доставки заказа
    /// </summary>
    public async Task UpdateDeliveryDateAsync(int orderId, DateTime deliveryDate)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.DeliveryDate = deliveryDate;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Рассчитать общую сумму заказа
    /// </summary>
    public async Task<decimal> CalculateOrderTotalAsync(int orderId)
    {
        return await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .SumAsync(oi => oi.Quantity * oi.PriceAtOrder * (1 - oi.DiscountAtOrder / 100));
    }

    /// <summary>
    /// Обновить общую сумму заказа
    /// </summary>
    private async Task UpdateOrderTotalAsync(int orderId)
    {
        var total = await CalculateOrderTotalAsync(orderId);
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.TotalAmount = total;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Получить товары в заказе в виде DTO
    /// </summary>
    public async Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(int orderId)
    {
        return await _context.OrderItems
            .Include(oi => oi.Product)
            .Where(oi => oi.OrderId == orderId)
            .Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product != null ? oi.Product.Name : null,
                ProductArticle = oi.Product != null ? oi.Product.Article : null,
                Quantity = oi.Quantity,
                PriceAtOrder = oi.PriceAtOrder,
                DiscountAtOrder = oi.DiscountAtOrder
            })
            .ToListAsync();
    }

    /// <summary>
    /// Удалить товар из заказа
    /// </summary>
    public async Task RemoveItemFromOrderAsync(int orderId, int productId)
    {
        var item = await _context.OrderItems
            .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ProductId == productId);

        if (item != null)
        {
            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();
            await UpdateOrderTotalAsync(orderId);
        }
    }

    /// <summary>
    /// Получить количество заказов
    /// </summary>
    public async Task<int> GetOrdersCountAsync()
    {
        return await _context.Orders.CountAsync();
    }

    /// <summary>
    /// Получить количество заказов по статусу
    /// </summary>
    public async Task<int> GetOrdersCountByStatusAsync(int statusId)
    {
        return await _context.Orders
            .Where(o => o.StatusId == statusId)
            .CountAsync();
    }

    /// <summary>
    /// Получить общее количество товаров в заказе
    /// </summary>
    public async Task<int> GetOrderItemsCountAsync(int orderId)
    {
        return await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .SumAsync(oi => oi.Quantity);
    }

    /// <summary>
    /// Проверить существование заказа
    /// </summary>
    public async Task<bool> OrderExistsAsync(int orderId)
    {
        return await _context.Orders.AnyAsync(o => o.Id == orderId);
    }

    /// <summary>
    /// Удалить заказ
    /// </summary>
    public async Task DeleteOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            // Удаляем все позиции заказа
            var items = _context.OrderItems.Where(oi => oi.OrderId == orderId);
            _context.OrderItems.RemoveRange(items);

            // Удаляем сам заказ
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Получить заказ как сущность (для внутреннего использования)
    /// </summary>
    public async Task<Order?> GetOrderEntityByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <summary>
    /// Получить заказ как сущность по номеру (для внутреннего использования)
    /// </summary>
    public async Task<Order?> GetOrderEntityByNumberAsync(int orderNumber)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    /// <summary>
    /// Получить все заказы как сущности (для внутреннего использования)
    /// </summary>
    public async Task<IEnumerable<Order>> GetOrderEntitiesAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}
