namespace OrderApi.Models;

public class OrderItem
{
    public required string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}