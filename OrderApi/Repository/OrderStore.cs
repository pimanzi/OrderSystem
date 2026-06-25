using OrderApi.Enums;
using OrderApi.Models;

namespace OrderApi.Repository;

public class OrderStore
{
    private readonly List<Order> _orders = new();

    public void Add(Order order)
    {
        _orders.Add(order);
    }

    public Order? GetById(string id)
    {
        return _orders
            .FirstOrDefault(o => o.Id == id);
    }

    public List<Order> GetAll()
    {
        return _orders;
    }

    public void Update(Order order)
    {
        var existing = GetById(order.Id);
        if (existing is not null)
        {
            existing.Status = order.Status;
        }
    }
}