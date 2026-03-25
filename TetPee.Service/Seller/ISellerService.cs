namespace TetPee.Service.Seller;

public interface ISellerService
{
    public Task<Base.Response.PageResult<Response.GetSellerResponse>> GetSellers(
        string? searchTerm,
        int pageIndex,
        int pageSize);

    public Task<Response.GetSellerByIdResponse> GetSellerById(Guid id);
    public Task<string> CreateSeller(Request.CreateSellerRequest request);
}