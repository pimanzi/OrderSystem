namespace Shared.Events;

public class OrderShippedEvent : BaseEvent
{
    public OrderShippedEvent()
    {
        EventType = "OrderShipped";
    }

    public string OrderId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;
}