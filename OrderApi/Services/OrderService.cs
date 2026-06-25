using OrderApi.Enums;
using OrderApi.Models;
using OrderApi.Publishers.Interfaces;
using OrderApi.Repository;
using OrderApi.Services.Interfaces;
using Shared.Events;

namespace OrderApi.Services;

public class OrderService : IOrderService
{
    private readonly OrderStore _store;
    private readonly IEventPublisher _publisher;

    public OrderService(OrderStore store, IEventPublisher publisher)
    {
        _store = store;
        _publisher = publisher;
    }

    public async  Task<Order> CreateOrder(string customer,
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
        await _publisher.PublishAsync(
            "order-events",
            new OrderPlacedEvent()
            {
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                Items = order.Items.Select(i =>
                    new OrderItemEvent
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList(),
                TotalAmount = order.TotalAmount
            });
            
            
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