using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backendfinal.Dtos;
using backendfinal.Services;

namespace backendfinal.Controllers;

[ApiController]
[Route("orders")]
[Authorize]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    /// <summary>
    /// Create a new customer order with one or more items.
    /// Status starts at CREATED.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var result = await orderService.CreateAsync(request);
        return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
    }

    /// <summary>
    /// Pick an order. Deducts inventory from a single bin.
    /// Order must be in CREATED status.
    /// Idempotent: picking an already-picked order returns 200 with no side effects.
    /// </summary>
    [HttpPost("{id:int}/pick")]
    [ProducesResponseType(typeof(PickOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Pick(int id)
    {
        var result = await orderService.PickAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Pack a picked order.
    /// Order must be in PICKED status.
    /// Idempotent: packing an already-packed order returns 200.
    /// </summary>
    [HttpPost("{id:int}/pack")]
    [ProducesResponseType(typeof(PackOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Pack(int id)
    {
        var result = await orderService.PackAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Ship a packed order.
    /// Order must be in PACKED status.
    /// Idempotent: shipping an already-shipped order returns 200.
    /// </summary>
    [HttpPost("{id:int}/ship")]
    [ProducesResponseType(typeof(ShipOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Ship(int id)
    {
        var result = await orderService.ShipAsync(id);
        return Ok(result);
    }
}