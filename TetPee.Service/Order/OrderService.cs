using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TetPee.Repository;
using TetPee.Repository.Entity;

namespace TetPee.Service.Order;

public class OrderService : IOrderService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;
    public OrderService(AppDbContext dbContext,  IHttpContextAccessor httpContext)
    {
        _dbContext = dbContext;
        _httpContext = httpContext;
    }

    public async Task<Response.CreateOrderResponse> CreateOrder(Request.CreateOrderRequest request)
    {
        var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
        
        var userIdGuid = Guid.Parse(userId!);
        
        //List Object -> List Guid -> Mapping List thì xài select
        
        var productIds = request.Product.Select(x => x.ProductId).Distinct().ToList();
        
        var query = _dbContext.Products.Where(x => productIds.Contains(x.Id));
        
        var productCount = await query.CountAsync();

        if (productCount != productIds.Count)
        {
            throw new Exception("Some products not found");
        }

        var result = await query.ToListAsync();
        
        decimal totalAmount = 0;

        foreach (var prod in result)
        {
            var quantity = request.Product.First(x => x.ProductId == prod.Id).Quantity;

            if (quantity <= 0)
            {
                throw new Exception($"Quantity of product {prod.Id} must be greater than 0");
            }
            totalAmount += quantity * prod.Price;
        }

        if (totalAmount <= 0)
        {
            throw new Exception("Total amount must be greater than 0");
        }

        var order = new Repository.Entity.Order()
        {
            Id = Guid.NewGuid(),
            UserId = userIdGuid,
            TotalAmount = totalAmount,
            Address = request.Address,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,

        };
        
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
        
        
        List<OrderDetail> orderDetails = new List<OrderDetail>();

        foreach (var prod in result)
        {
            var quantity = request.Product.First(x => x.ProductId == prod.Id).Quantity;
            
            
            var orderDetail = new OrderDetail()
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = prod.Id,
                Quantity = quantity,
                UnitPrice = prod.Price
            };
            
            orderDetails.Add(orderDetail);
        }

        if (orderDetails.Any())
        {
            _dbContext.AddRange(orderDetails);
            await _dbContext.SaveChangesAsync();
                
        }

        string description = $"TETPEE - {order.Id}";
            

        var response = new Response.CreateOrderResponse()
        {
            OrderId = order.Id,
            TotalAmount = totalAmount,
            BankName = "VPBank",
            BankAccount = "AGBSPEKUZUH8",
            Description = description,
            QRCode = ""
        };
        
        string qrCode = $"https://qr.sepay.vn/img?" +
                        $"acc={response.BankAccount}&" +
                        $"bank={response.BankName}&" +
                        $"amount={(int)totalAmount}&" +
                        $"des={description}&" +
                        $"template=gronly";

        response.QRCode = qrCode;
        return response;
    }

    public async Task SepayWebhookHandler(Request.SepayWebhookRequest request)
    {
        
        var description = request.Code; // ORDERID

        var raw = description. Replace("TETPEE", "");

        Guid? orderId = null;

        if (raw.Length == 32)
        {

            var formatted  = $"{raw. Substring( startIndex:0, length: 8)}-" +
                                   $"{raw. Substring(startIndex: 8, length: 4)}-" +
                                   $"{raw.Substring(startIndex:12, length: 4)}-" +
                                   $"{raw.Substring(startIndex: 16, length: 4)}-" +
                                   $"{raw.Substring(startIndex: 20, length: 12)}";

            if (Guid.TryParse(formatted, out var guid))
            {
                orderId = guid;
            }
               

        }else
        {
            throw new Exception("Invalid description format");
        }
        
        var query = _dbContext.Orders.Where(x => x.Id == orderId);
        var order =  await query.FirstOrDefaultAsync();

        if (order == null)
        {
            throw new Exception("Order not found");
        }

        if (order.Status != "Pending")
        {
            throw new Exception("Order already in progress");
        }

        if (order.TotalAmount != request.TransferAmount)
        {
            throw new Exception("Invalid transfer amount");
        }
            
        order.Status = "Completed";
        _dbContext.Update(order);
        await _dbContext.SaveChangesAsync();
        
    }
}

// Bản chất của sepay
// Là 1 thằng lắng nghe tất cả các giao dịch của mình trong tài khoản
// và mình có thể làm 1 thứ, nếu có giao dịch nào chuyênr đến thì gọi 1 api callback 
// và ko phải giao dịch nào cũng là giao dịch của hệ thống mình
    // Giao dịch trả tiền nợ từ bạn A -> Call api
    // Giao dịch mua hàng từ hệ thống TetPee -> Call api
    // Giao dịch trả tiền cổ tức -> Call api

    //     