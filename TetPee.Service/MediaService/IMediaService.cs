using Microsoft.AspNetCore.Http;
namespace TetPee.Service.MediaService;

public interface IMediaService
{
    public Task<string> UploadImageAsync(IFormFile file);
}