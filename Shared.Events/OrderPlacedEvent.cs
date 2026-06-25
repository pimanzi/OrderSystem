namespace Shared.Events;

public class OrderPlacedEvent : BaseEvent
{
    public OrderPlacedEvent()
    {
        EventType = "OrderPlaced";
    }

    public string OrderId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderItemEvent> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

public class OrderItemEvent
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}