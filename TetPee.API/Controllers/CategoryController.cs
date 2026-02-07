

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
    public CategoryController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public IActionResult GetCategories([FromQuery] string? searchTerm)
    {
        var categories = _dbContext.Categories.ToList();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public IActionResult GetCategory(Guid id)
    {
        return Ok("Get Category by id");
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