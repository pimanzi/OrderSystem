using OrderApi.Enums;

namespace OrderApi.Models;

public class Order
{
    public string Id { get; set; }
        = Guid.NewGuid().ToString();

    public required string  CustomerName { get; set; }
    public List<OrderItem> Items { get; set; }
        = new();

    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
        = OrderStatus.Placed;

    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;
}