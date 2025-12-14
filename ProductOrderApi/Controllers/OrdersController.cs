using Microsoft.AspNetCore.Mvc;
using ProductOrderApi.Models;

namespace ProductOrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private static List<Order> _orders = new();
    private static int _nextId = 1;

    [HttpGet]
    public ActionResult<IEnumerable<Order>> GetOrders()
    {
        return Ok(_orders);
    }

    [HttpGet("{id}")]
    public ActionResult<Order> GetOrder(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    [HttpPost]
    public ActionResult<Order> CreateOrder(Order order)
    {
        order.Id = _nextId++;
        order.OrderDate = DateTime.UtcNow;
        order.Status = OrderStatus.Pending;
        
        // Calculate total amount
        order.TotalAmount = order.Items.Sum(item => item.Price * item.Quantity);
        
        _orders.Add(order);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateOrder(int id, Order order)
    {
        var existingOrder = _orders.FirstOrDefault(o => o.Id == id);
        if (existingOrder == null)
        {
            return NotFound();
        }

        existingOrder.CustomerName = order.CustomerName;
        existingOrder.CustomerEmail = order.CustomerEmail;
        existingOrder.Status = order.Status;
        existingOrder.Items = order.Items;
        existingOrder.TotalAmount = order.Items.Sum(item => item.Price * item.Quantity);

        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public IActionResult UpdateOrderStatus(int id, [FromBody] OrderStatus status)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        order.Status = status;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteOrder(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        _orders.Remove(order);
        return NoContent();
    }
}
