using Microsoft.EntityFrameworkCore;
using TetPee.Repository;

namespace TetPee.Service.Category;

public class CategoryService: ICategoryService
{
    private readonly AppDbContext _dbContext;
    
    
    public CategoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task <List<Response.GetCategoryResponse>>  GetAllCategories()
    {
        var query = _dbContext.Categories.Where(x => true);
        query = query.OrderBy(x => x.Name);
        var selectedQuery = query.Select(x => new Response.GetCategoryResponse()
        {
            Id = x.Id,
            Name = x.Name
        });
        var result = await selectedQuery.ToListAsync();
        return result;
    }

    public async Task<List<Response.GetCategoryResponse>> GetAllChildrenCategoriesByParentId(Guid id)
    {
        var query = _dbContext.Categories.Where(x => x.ParentId == id);
        query = query.OrderBy(x => x.Name);
        var selectedQuery = query.Select(x => new Response.GetCategoryResponse()
        {
            Id = x.Id,
            Name = x.Name
        });
        var result = await selectedQuery.ToListAsync();
        return result;
    }
}