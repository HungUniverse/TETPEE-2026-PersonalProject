using Microsoft.AspNetCore.Mvc;
using TetPee.Service.Identity;
using TetPee.Service.Models;

namespace TetPee.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController: ControllerBase
{
    private readonly IIdentityService _identityService;

    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    [HttpGet("Login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _identityService.Login( email, password);
        return Ok(ApiResponseFactory.SuccessResponse(result, "Login successfully", HttpContext.TraceIdentifier));
    }
}