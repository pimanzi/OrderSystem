
using OrderApi.Models;

namespace OrderApi.Dtos;

public class CreateOrderDto
{
    public  required string CustomerName { get; set; } 
    public required List<OrderItem> Items { get; set; } 
}