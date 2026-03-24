using Microsoft.EntityFrameworkCore;
using TetPee.Repository;

namespace TetPee.Service.User;

public class UserService: IUserService
{
    private readonly AppDbContext _dbContext;
    
    
    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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
}