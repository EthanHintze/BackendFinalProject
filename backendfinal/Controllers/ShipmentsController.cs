using backendfinal.Dtos;
using backendfinal.Exceptions;
using backendfinal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backendfinal.Controllers;

[ApiController]
[Route("shipments")]
[Authorize]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentsController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    /// <summary>
    /// Create a new shipment linked to a purchase order.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ShipmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var response = await _shipmentService.CreateShipmentAsync(request);
        return CreatedAtAction(nameof(CreateShipment), new { id = response.Id }, response);
    }

    /// <summary>
    /// Receive a shipment by ID. Updates inventory.
    /// Cannot be received more than once (409 if already received).
    /// </summary>
    [HttpPost("{id:int}/receive")]
    [ProducesResponseType(typeof(ReceiveShipmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReceiveShipment(int id)
    {
        var request = new ReceiveShipmentRequest { ShipmentId = id };
        var response = await _shipmentService.ReceiveShipmentAsync(request);
        return Ok(response);
    }
}