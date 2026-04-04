namespace TetPee.Service.User;

public interface IUserService
{
    public Task<Base.Response.PageResult<Response.GetUserResponse>> GetUsers(
        string? searchTerm,
        int pageIndex,
        int pageSize);

    public Task<Response.GetUserResponse?> GetUserById(Guid id);
    
    public Task<string> CreateUser(Request.CreateUserRequest request, CancellationToken  cancellationToken);
}