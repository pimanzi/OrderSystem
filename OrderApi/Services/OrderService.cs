using OrderApi.Enums;
using OrderApi.Models;
using OrderApi.Repository;
using OrderApi.Services.Interfaces;

namespace OrderApi.Services;

public class OrderService : IOrderService
{
    private readonly OrderStore _store;

    public OrderService(OrderStore store)
    {
        _store = store;
    }

    public Order CreateOrder(string customer,
        List<OrderItem> items)
    {
        var order = new Order
        {
            CustomerName = customer,
            Items = items,
            TotalAmount = items
                .Sum(i => i.Price * i.Quantity)
        };

        _store.Add(order);
        return order;
    }

    public (bool Success, string? Error) ConfirmOrder(string id)
    {
        var order = _store.GetById(id);

        if (order is null)
            return (false, $"Order {id} not found");

        if (order.Status != OrderStatus.Placed)
            return (false,
                $"Cannot confirm order with " +
                $"status {order.Status}");

        order.Status = OrderStatus.Confirmed;
        _store.Update(order);
        return (true, null);
    }

    public (bool Success, string? Error) ShipOrder(string id)
    {
        var order = _store.GetById(id);

        if (order is null)
            return (false, $"Order {id} not found");

        if (order.Status != OrderStatus.Confirmed)
            return (false,
                $"Cannot ship order with " +
                $"status {order.Status}");

        order.Status = OrderStatus.Shipped;
        _store.Update(order);
        return (true, null);
    }

    public Order? GetById(string id)
        => _store.GetById(id);

    public List<Order> GetAll()
        => _store.GetAll();
}