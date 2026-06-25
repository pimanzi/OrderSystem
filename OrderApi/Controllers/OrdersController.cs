using Microsoft.AspNetCore.Mvc;
using OrderApi.Dtos;
using OrderApi.Models;
using OrderApi.Services.Interfaces;

namespace OrderApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public ActionResult<List<Order>> GetAll()
    {
        var orders = _orderService.GetAll();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public ActionResult<Order> GetById(string id)
    {
        var order = _orderService.GetById(id);

        if (order is null)
            return NotFound($"Order {id} not found");

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create(
        [FromBody] CreateOrderDto request)
    {
        var order = await _orderService.CreateOrder(
            request.CustomerName,
            request.Items);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }

    [HttpPut("{id}/confirm")]
    public async Task<ActionResult> Confirm(string id)
    {
        var (success, error) =
            await _orderService.ConfirmOrder(id);
        if (!success)
            return BadRequest(error);

        return Ok($"Order {id} confirmed!");
    }
    [HttpPut("{id}/ship")]
    public async Task<ActionResult> Ship(string id)
    {
        var (success, error) =
            await _orderService.ShipOrder(id);
        if (!success)
            return BadRequest(error);

        return Ok($"Order {id} shipped!");
    }
}