using backendfinal.Dtos;
using backendfinal.Services;
using Microsoft.AspNetCore.Mvc;

namespace backendfinal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderService _purchaseOrderService;

    public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService = purchaseOrderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (request.DateOrdered > DateTime.UtcNow)
        {
            ModelState.AddModelError(nameof(request.DateOrdered), "DateOrdered cannot be in the future.");
            return ValidationProblem(ModelState);
        }

        var purchaseOrder = await _purchaseOrderService.CreatePurchaseOrderAsync(request);

        var response = new PurchaseOrderResponse(purchaseOrder.Id, purchaseOrder.DateOrdered);
        return CreatedAtAction(nameof(GetPurchaseOrderById), new { id = purchaseOrder.Id }, response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPurchaseOrderById(int id)
    {
        var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
        if (purchaseOrder is null)
        {
            return NotFound();
        }

        var response = new PurchaseOrderResponse(purchaseOrder.Id, purchaseOrder.DateOrdered);
        return Ok(response);
    }
}
