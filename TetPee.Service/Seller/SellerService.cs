using Microsoft.EntityFrameworkCore;
using TetPee.Repository;
using TetPee.Service.MailService;

namespace TetPee.Service.Seller;

public class SellerService: ISellerService
{
    private readonly AppDbContext _dbContext;
    private readonly IMailService _mailService;
    
    public SellerService(AppDbContext dbContext,  IMailService mailService)
    {
        _dbContext = dbContext;
        _mailService = mailService;
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
        var isExistUser = await _dbContext.Users.AnyAsync(x => x.Email == request.Email);
        if (isExistUser)
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

        var seller = new Repository.Entity.Seller()
        {
            CompanyAddress = request.CompanyAddress,
            CompanyName = request.CompanyName,
            TaxCode = request.TaxCode,
            User = user,
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            _dbContext.Users.Add(user);
            _dbContext.Sellers.Add(seller);

            var result = await _dbContext.SaveChangesAsync();
            if (result <= 0)
            {
                await transaction.RollbackAsync();
                return Message.FailToAddSeller;
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        try
        {
            await _mailService.SendMail(new MailContent()
            {
                To = request.Email,
                Subject = "Welcome to TetPee Seller Center",
                Body = BuildSellerWelcomeMailBody(request)
            });
        }
        catch
        {
            // Do not fail seller creation if email provider is temporarily unavailable.
        }

        return "Add Seller successfully";
    }

    private static string BuildSellerWelcomeMailBody(Request.CreateSellerRequest request)
    {
        var safeFirstName = System.Net.WebUtility.HtmlEncode(request.FirstName);
        var safeCompanyName = System.Net.WebUtility.HtmlEncode(request.CompanyName);
        var safeCompanyAddress = System.Net.WebUtility.HtmlEncode(request.CompanyAddress);
        var safeTaxCode = System.Net.WebUtility.HtmlEncode(request.TaxCode);

        return $$"""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width,initial-scale=1.0">
    <title>Welcome to TetPee</title>
    <style>
        body { margin: 0; padding: 0; background: #f3f4f6; font-family: Arial, Helvetica, sans-serif; }
        .container { width: 100%; background: #f3f4f6; padding: 28px 12px; }
        .card { max-width: 640px; margin: 0 auto; background: #ffffff; border-radius: 18px; overflow: hidden; box-shadow: 0 14px 35px rgba(17, 24, 39, 0.12); }
        .hero { padding: 28px 30px; background: linear-gradient(135deg, #0f766e 0%, #0369a1 100%); color: #ffffff; }
        .brand { font-size: 24px; font-weight: 700; letter-spacing: 1px; margin: 0 0 8px; }
        .hero-title { margin: 0; font-size: 22px; line-height: 1.35; }
        .content { padding: 26px 30px 30px; color: #111827; }
        .content p { margin: 0 0 14px; line-height: 1.65; font-size: 15px; }
        .info-box { margin: 18px 0 20px; border: 1px solid #e5e7eb; border-radius: 12px; overflow: hidden; }
        .info-row { display: block; padding: 12px 14px; border-bottom: 1px solid #eef2f7; }
        .info-row:last-child { border-bottom: none; }
        .label { color: #6b7280; font-size: 12px; text-transform: uppercase; letter-spacing: 0.08em; display: block; margin-bottom: 5px; }
        .value { color: #111827; font-size: 15px; font-weight: 600; word-break: break-word; }
        .button-wrap { margin: 22px 0 10px; }
        .button { display: inline-block; background: #0f766e; color: #ffffff !important; text-decoration: none; font-weight: 700; padding: 12px 20px; border-radius: 10px; }
        .footer { padding: 0 30px 30px; color: #6b7280; font-size: 12px; line-height: 1.6; }
        @media (max-width: 600px) {
            .hero { padding: 24px 20px; }
            .content { padding: 22px 20px 24px; }
            .footer { padding: 0 20px 24px; }
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="card">
            <div class="hero">
                <p class="brand">TetPee</p>
                <h1 class="hero-title">Seller account created successfully</h1>
            </div>

            <div class="content">
                <p>Hi {{safeFirstName}},</p>
                <p>Welcome to TetPee Seller Center. Your seller profile has been created and is ready for the next setup steps.</p>

                <div class="info-box">
                    <div class="info-row">
                        <span class="label">Company Name</span>
                        <span class="value">{{safeCompanyName}}</span>
                    </div>
                    <div class="info-row">
                        <span class="label">Company Address</span>
                        <span class="value">{{safeCompanyAddress}}</span>
                    </div>
                    <div class="info-row">
                        <span class="label">Tax Code</span>
                        <span class="value">{{safeTaxCode}}</span>
                    </div>
                </div>

                <p>Next step: log in and complete your product information so your store can start selling quickly.</p>

                <div class="button-wrap">
                    <a href="https://tetpee.vn" class="button">Go to TetPee</a>
                </div>
            </div>

            <div class="footer">
                This email was sent automatically by TetPee. If you did not request this account, please contact support immediately.
            </div>
        </div>
    </div>
</body>
</html>
""";
    }
}