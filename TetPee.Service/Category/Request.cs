namespace TetPee.Service.Category;

public class Request
{
    public class CreateCategoryRequest
    {
        public Guid? ParentId { get; set; } = Guid.Empty;
        public required string Name { get; set; }
    }

    public class UpdateCategoryRequest:  CreateCategoryRequest
    {
        public Guid Id { get; set; }
    }
}