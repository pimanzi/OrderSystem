// Shared.Events/OrderConfirmedEvent.cs
namespace Shared.Events;

public class OrderConfirmedEvent : BaseEvent
{
    public OrderConfirmedEvent()
    {
        EventType = "OrderConfirmed";
    }

    public string OrderId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}