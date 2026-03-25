namespace TetPee.Service.Product;

public interface IProductService
{
    public Task<string> CreateProduct(Request.CreateProductRequest createProductRequest);

}