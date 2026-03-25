
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TetPee.Api.Extensions;
using TetPee.Repository;
using TetPee.Service.Seller;

namespace TetPee.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SellerController: ControllerBase
{
    private readonly AppDbContext _dbContext;
    
    //Cái này nâng cao
    private readonly ISellerService _sellerService;

    public SellerController(AppDbContext dbContext, ISellerService sellerService)
    {
        _dbContext = dbContext;
        _sellerService = sellerService;
    }
    
    
    [Authorize(Policy = JwtExtensions.AdminPolicy)]
    [HttpGet("")]
    public async Task<IActionResult> GetSeller( string? searchTerm, int pageSize = 10, int pageIndex = 1)
    {
        var sellers = await _sellerService.GetSellers(searchTerm, pageSize, pageIndex);
        return Ok(sellers);
        
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSellerById(Guid id)
    {
        var seller = await _sellerService.GetSellerById(id);
        return Ok(seller);
    }
}