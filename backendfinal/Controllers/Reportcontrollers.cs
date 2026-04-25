using backendfinal.Dtos;
using backendfinal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backendfinal.Controllers;

// ═══════════════════════════════════════════════════════════════════
// GET /inventory[?productId=]
// ═══════════════════════════════════════════════════════════════════

[ApiController]
[Route("inventory")]
[Authorize]
public class InventoryReportController(IReportService reportService) : ControllerBase
{
    /// <summary>
    /// Returns current inventory levels across all bins.
    /// Reflects all shipment receives and order picks.
    /// Optionally filter to a single product with ?productId=
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(InventoryReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInventory([FromQuery] int? productId)
    {
        var result = await reportService.GetInventoryAsync(productId);
        return Ok(result);
    }
}

// ═══════════════════════════════════════════════════════════════════
// GET /orders/{id}
// ═══════════════════════════════════════════════════════════════════

[ApiController]
[Route("orders")]
[Authorize]
public class OrdersReportController(IReportService reportService) : ControllerBase
{
    /// <summary>
    /// Returns the current status and details of a customer order.
    /// Includes shipping info when the order has been shipped.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(int id)
    {
        var result = await reportService.GetOrderStatusAsync(id);
        return Ok(result);
    }
}