

using TetPee.Repository;
using TetPee.Repository.Entity;
using Microsoft.AspNetCore.Mvc;
using TetPee.Service.Category;


namespace TetPee.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController:  ControllerBase
{
    public readonly AppDbContext _dbContext;
    private readonly ICategoryService _categoryService;

    public CategoryController (AppDbContext dbContext, ICategoryService categoryService)
    {
        _dbContext = dbContext;
        _categoryService = categoryService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategories();
        return Ok(categories);
    }

    [HttpGet("{parentId}/childrens")]
    public async Task<IActionResult> GetAllChildrenCategoryById(Guid parentId)
    {
        var childCategories = await _categoryService.GetAllChildrenCategoriesByParentId(parentId);
        return Ok(childCategories);
    }
    
    [HttpPost("")]
    public IActionResult CreateCategory([FromBody] Request.CreateCategoryRequest request)
    {
        
        var category = new Category()
        {
            ParentId =  request.ParentId,
            Name = request.Name,
        };
        
        _dbContext.Categories.Add(category);
        _dbContext.SaveChanges();
        return Ok(category);
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateCategory( Guid id, [FromBody] Request.UpdateCategoryRequest request)
    {
        return Ok("Update Category by id");
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteCategory(Guid id)
    {
        return Ok("Delete Category by id");
    }
}