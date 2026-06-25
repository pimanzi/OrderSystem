using OrderApi.Models;

namespace OrderApi.Services.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrder(string customerId,
        List<OrderItem> items);
    (bool Success, string? Error) ConfirmOrder(string id);
    (bool Success, string? Error) ShipOrder(string id);
    Order? GetById(string id);
    List<Order> GetAll();
}