using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TetPee.Service.Models;
using TetPee.Service.Order;

namespace TetPee.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class Order: ControllerBase
{
    
    private readonly IOrderService _orderService;

    public Order(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpPost("")]
    public async Task<IActionResult> CreateOrder(Request.CreateOrderRequest request)
    {
        var result = await _orderService.CreateOrder(request);
        return Ok(ApiResponseFactory.SuccessResponse(result, "Order created", HttpContext.TraceIdentifier));
    }
    
    [HttpPost("Sepay/Webhook")]
    public async Task<IActionResult> SepayWebhook(Request.SepayWebhookRequest request)
    {
        
        return Ok(ApiResponseFactory.SuccessResponse(null,"Webhook created", HttpContext.TraceIdentifier));
    }
    
}