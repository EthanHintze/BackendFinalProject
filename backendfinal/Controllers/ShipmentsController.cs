using backendfinal.Dtos;
using backendfinal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace backendfinal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentsController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveShipment([FromBody] ReceiveShipmentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var response = await _shipmentService.ReceiveShipmentAsync(request);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Shipment not found." });
        }
        catch (ShipmentAlreadyReceivedException)
        {
            return Conflict(new { Message = "Shipment has already been received." });
        }
        catch (ShipmentValidationException ex)
        {
            var errors = new ModelStateDictionary();
            errors.AddModelError(string.Empty, ex.Message);
            return ValidationProblem(errors);
        }
    }
}
