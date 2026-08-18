using Microsoft.AspNetCore.Http;

namespace LegendCraft_Backend.Services
{
    public interface IImageStorageService
    {
        Task<string> SaveImageAsync(IFormFile file, string folderName);
        Task DeleteImageAsync(string imageUrl);
    }
}
