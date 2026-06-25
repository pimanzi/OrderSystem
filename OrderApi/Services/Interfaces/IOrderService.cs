using OrderApi.Models;

namespace OrderApi.Services.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrder(string customerId,
        List<OrderItem> items);
    Task<(bool Success, string? Error)> ConfirmOrder(string id);
    Task<(bool Success, string? Error)> ShipOrder(string id);
    Order? GetById(string id);
    List<Order> GetAll();
}