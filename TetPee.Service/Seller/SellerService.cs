using Microsoft.EntityFrameworkCore;
using TetPee.Repository;

namespace TetPee.Service.Seller;

public class SellerService: ISellerService
{
    private readonly AppDbContext _dbContext;
    
    
    public SellerService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Base.Response.PageResult<Response.GetSellerResponse>> GetSellers(string? searchTerm, int pageSize, int pageIndex)
    {
        var query = _dbContext.Sellers.Where(u => true);
        if (searchTerm != null)
        {
            query = query.Where(u =>
                u.User.FirstName.Contains(searchTerm) || 
                u.User.LastName.Contains(searchTerm));
        }

        //query = query.Include(u => u.User);
        
        query = query.OrderBy(x => x.User.Email);
        
        query = query
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize);
        
        

        var selectedQuery = query.Select(x => new Response.GetSellerResponse()
        {
            Email = x.User.Email,
            FirstName = x.User.FirstName,
            LastName = x.User.LastName,
            ImageUrl = x.User.ImageUrl,
            Taxcode = x.TaxCode,
            CompanyName = x.CompanyName,
        });
        
        var listResult  = await selectedQuery.ToListAsync();
        var totalItems = listResult.Count();

        var result = new Base.Response.PageResult<Response.GetSellerResponse>()
        {
            Items = listResult,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItems = totalItems,
        };
        return result;
    }

    public async Task<Response.GetSellerByIdResponse> GetSellerById(Guid id)
    {
        var query = _dbContext.Sellers.Where(u => u.Id == id);
        //query = query.Include(u => u.User);
        var selectedQuery = query.Select(x => new Response.GetSellerByIdResponse()
        {
            Email = x.User.Email,
            FirstName =  x.User.FirstName,
            LastName = x.User.LastName,
            ImageUrl = x.User.ImageUrl,
            PhoneNumber = x.User.PhoneNumber,
            Address = x.User.Address,
            Taxcode = x.TaxCode,
            CompanyName = x.CompanyName,
            CompanyAddress = x.CompanyAddress,
            
        });
        var result = await selectedQuery.FirstOrDefaultAsync();
        return result;
    }
    
    public async Task<string> CreateSeller(Request.CreateSellerRequest request)
    {
        var existingUserQuery = _dbContext.Users.Where(x => x.Email == request.Email);
        
        bool isExistUser = await existingUserQuery.AnyAsync();
        
        if(isExistUser)
        {
            throw new Exception(Message.UserExistWithMail);
        }
        
        var user = new Repository.Entity.User()
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            HashedPassword = request.Password,
            Role = "Seller"
        };

        _dbContext.Add(user);

        var result = await _dbContext.SaveChangesAsync();
        
        if (result > 0)
        {
            var seller = new Repository.Entity.Seller()
            {
                CompanyAddress = request.CompanyAddress,
                CompanyName = request.CompanyName,
                TaxCode = request.TaxCode,
                UserId = user.Id,
            };
            
            _dbContext.Add(seller);
            
            var sellerResult = await _dbContext.SaveChangesAsync();

            if (sellerResult > 0) return "Add Seller successfully";
            
            return Message.FailToAddSeller;
        }
        
        return Message.FailToAddSeller;
    }
}