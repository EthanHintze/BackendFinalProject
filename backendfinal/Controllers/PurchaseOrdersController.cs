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

        var purchaseOrder = await _purchaseOrderService.CreatePurchaseOrderAsync(request);

        var response = new PurchaseOrderResponse(
            purchaseOrder.Id,
            purchaseOrder.DateOrdered,
            purchaseOrder.Status,
            purchaseOrder.OrderItems.Select(item => new PurchaseOrderItemResponse(item.ItemId ?? 0, item.Quantity ?? 0)).ToList());

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

        var response = new PurchaseOrderResponse(
            purchaseOrder.Id,
            purchaseOrder.DateOrdered,
            purchaseOrder.Status,
            purchaseOrder.OrderItems.Select(item => new PurchaseOrderItemResponse(item.ItemId ?? 0, item.Quantity ?? 0)).ToList());

        return Ok(response);
    }
}
