using Microsoft.EntityFrameworkCore;
using TetPee.Repository;
using TetPee.Service.MailService;
using TetPee.Service.MediaService;

namespace TetPee.Service.User;

public class UserService: IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IMailService _emailService;
    private readonly IMediaService _mediaService;
    
    
    public UserService(AppDbContext dbContext,  IMailService emailService,  IMediaService mediaService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _mediaService = mediaService;
    }
    
    public async Task<Base.Response.PageResult<Response.GetUserResponse>> GetUsers(
        string? searchTerm, 
        int pageSize, 
        int pageIndex)
    {
        var query = _dbContext.Users.Where(u => true);
        if (searchTerm != null)
        {
            query = query.Where(u =>
                u.Email.Contains(searchTerm) || 
                u.FirstName.Contains(searchTerm) || 
                u.LastName.Contains(searchTerm));
        }
        
        query = query.OrderBy(x => x.Email);
        
        query = query
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize);
        
        

        var selectedQuery = query.Select(x => new Response.GetUserResponse()
        {
            Id = x.Id,
            Email = x.Email,
            Firstname =  x.FirstName,
            Lastname = x.LastName,
            ImageUrl = x.ImageUrl,
            PhoneNumber = x.PhoneNumber,
            Address = x.Address,
            Role = x.Role,
            
        });
        
        var listResult  = await selectedQuery.ToListAsync();
        var totalItems = listResult.Count();

        var result = new Base.Response.PageResult<Response.GetUserResponse>()
        {
            Items = listResult,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItems = totalItems,
        };
        return result;
    }

    public async Task<Response.GetUserResponse> GetUserById(Guid id)
    {
        var query = _dbContext.Users.Where(u => u.Id == id);
        var selectedQuery = query.Select(x => new Response.GetUserResponse()
        {
            Id = x.Id,
            Email = x.Email,
            Firstname =  x.FirstName,
            Lastname = x.LastName,
            ImageUrl = x.ImageUrl,
            PhoneNumber = x.PhoneNumber,
            Address = x.Address,
            Role = x.Role,
            
        });
        var result = await selectedQuery.FirstOrDefaultAsync();
        return result;
    }

    public async Task<string> CreateUser(Request.CreateUserRequest request, CancellationToken cancellationToken)
    {
        var checkUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if(checkUser != null) throw new Exception("Email already exists");
        
        var user = new Repository.Entity.User()
        {
            Email = request.Email,
            FirstName = request.Firstname,
            LastName = request.Lastname,
            HashedPassword = request.Password // Chưa hash, chỉ demo
        };
        
        if(request.Avatar != null)
        {
            var media = await _mediaService.UploadAsync(request.Avatar);
            user.ImageUrl = media;
        }
        
        _dbContext.Users.Add(user);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return "User created";
    }
}