using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TetPee.Repository;
using TetPee.Service.JwtService;

namespace TetPee.Service.Identity;

public class IdentityService: IIdentityService
{
    private readonly IJwtService _jwtService;
    private readonly AppDbContext _dbContext;
    private readonly JwtOptions _jwtOption = new();
    
    
    public IdentityService(IJwtService jwtService,AppDbContext dbContext, IConfiguration configuration)
    {
        _jwtService = jwtService;
        _dbContext = dbContext;
        configuration.GetSection(nameof(JwtOptions)).Bind(_jwtOption);
    }
    
    public async Task<Response.IdentityResponse> Login(string email, string password)   
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (user.HashedPassword != password)
        {
            throw new Exception("Invalid password");
        }
        
        // User này chắc chắn là tôi
        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("Email", user.Email),
            new Claim("Role", user.Role),
            new Claim(ClaimTypes.Role, user.Role),
            //Phải có claim này  để phân quyền cho các API endpoint, nếu thiếu claim này thiì seẽ khoông phân quyền đc
            new Claim(ClaimTypes.Expired, DateTimeOffset.UtcNow.AddMinutes(_jwtOption.ExpireMinutes).ToString())
        };

        var token = _jwtService.GenerateAccessToken(claims);

        var result = new Response.IdentityResponse()
        {
            AccessToken = token,

        };
        return result;

    }
}