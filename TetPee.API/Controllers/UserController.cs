using Microsoft.AspNetCore.Mvc;
using TetPee.Repository;
using TetPee.Repository.Entity;
using TetPee.Service.MediaService;
using TetPee.Service.User;

namespace TetPee.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController: ControllerBase
{
    private readonly AppDbContext _dbContext;
    
    //Cái này nâng cao
    private readonly IUserService _userService;
    private readonly IMediaService _mediaService;

    public UserController(AppDbContext dbContext, IUserService userService,  IMediaService mediaService)
    {
        _dbContext = dbContext;
        _userService = userService;
        _mediaService = mediaService;
    }

    //HTTP Method: GET, POST, PUT, DELETE, PATCH
    // PARAM: Query string, Path param, body Param
    
    //Query string: http://localhost:5000/User?name=abc&age=20
    // name và age là query string
    // query string nằm sau dấu ?
    
    // Path (Route) param: http://localhost:5000/User/123
    // 123 là path param hoặc route param
    // Path param nằm trong đg dẫn
    
    // GET là ko có body
    // POST, PUT, PATCH ko có body
    
    //Tại sao phải dùng body: tránh để ộ những thông tin ko mong muốn
    //Ví dụ: Username, Pass
    //Không thể http://localhost:5000/login?username=abc&password=123
    
    //Chuẩn REST FULL API
    // get all users: GET  http://localhost:5000/User
    // create all users: POST http://localhost:5000/User
    // get user by id: GET  //http://localhost:5000/User/{id}
    // update user by id: PUT  //http://localhost:5000/User/{id}
    // delete user by id: DELETE  //http://localhost:5000/User/{id}
    
    [HttpGet("")]
    public async Task<IActionResult> GetUsers( string? searchTerm, int pageSize = 10, int pageIndex = 1)
    {
        var users = await _userService.GetUsers(searchTerm, pageSize, pageIndex);
        return Ok(users);
        
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserById(id);
        return Ok(user);
    }
    
    [HttpPost("")]
    public async Task<IActionResult> CreateUsers([FromForm] Request.CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = new User()
        {
            Email = request.Email,
            FirstName = request.Firstname,
            LastName = request.Lastname,
            HashedPassword = request.Password // Chưa hash, chỉ demo
        };
        
        if(request.Avatar != null)
        {
            var media = await _mediaService.UploadImageAsync(request.Avatar);
            user.ImageUrl = media;
        }
        
        _dbContext.Users.Add(user);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Ok("Create user successfully");
    }
    
    [HttpPut("id")]
    public IActionResult UpdateUserById(Guid id, Request.UpdateUserRequest request)
    {
        Console.WriteLine(request);
        return Ok("Update User");
    }
    
    [HttpDelete("id")]
    public IActionResult DeleteUserById(Guid id)
    {
        return Ok("Delete User");
    }
}