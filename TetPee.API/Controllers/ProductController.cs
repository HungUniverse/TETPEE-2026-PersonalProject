using Microsoft.AspNetCore.Mvc;
using TetPee.Api.Extensions;
using TetPee.Service.Product;
using Microsoft.AspNetCore.Authorization;
namespace TetPee.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController: ControllerBase
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [Authorize(Policy = JwtExtensions.SellerPolicy)]
    [HttpPost("CreateProduct")]
    public async Task<IActionResult> CreateProduct(Request.CreateProductRequest createProductRequest)
    {
        var product = await _productService.CreateProduct(createProductRequest);
        return Ok(product);
    }
}