namespace TetPee.Service.Category;

public interface ICategoryService
{
    public Task <List<Response.GetCategoryResponse>> GetAllCategories ();
    public Task <List<Response.GetCategoryResponse>> GetAllChildrenCategoriesByParentId (Guid id);
}