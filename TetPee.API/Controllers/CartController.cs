using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TetPee.Api.Extensions;
using TetPee.Service.Cart;
using TetPee.Service.Models;

namespace TetPee.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CartController: ControllerBase
{
    private readonly ICartService _cartService;
    public  CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [Authorize(Policy = JwtExtensions.UserPolicy)]
    [HttpPost("Cart")]
    public async Task<IActionResult> CreateCart()
    {
        await _cartService.CreateCart();
        return Ok(ApiResponseFactory.SuccessResponse(null, "Cart retrieved", HttpContext.TraceIdentifier));
    }
    
    [Authorize(Policy = JwtExtensions.UserPolicy)]
    [HttpPost("AddProductToCart")]
    public async Task<IActionResult> AddProductToCart(Request.AddProductToCartRequest request)
    {
        await _cartService.AddProductToCart(request);
        return Ok(ApiResponseFactory.SuccessResponse("Successfully", "Add product to cart retrieved", HttpContext.TraceIdentifier));
    }
    
    [Authorize(Policy = JwtExtensions.UserPolicy)]
    [HttpDelete("Delete")]
    public async Task<IActionResult> RemoveProductFromCart(Request.RemoveProductFromCartRequest request)
    {
        await _cartService.RemoveProductFromCart(request);
        return Ok(ApiResponseFactory.SuccessResponse("Successfully", "remove product to cart retrieved", HttpContext.TraceIdentifier));
    }
    
    [Authorize(Policy = JwtExtensions.UserPolicy)]
    [HttpGet("Get")]
    public async Task<IActionResult> GetCart()
    {
        var result = await _cartService.GetCart();
        return Ok(ApiResponseFactory.SuccessResponse(result, "Get card", HttpContext.TraceIdentifier));
    }
    
}