using Microsoft.AspNetCore.Http;
using System.IO;

namespace LegendCraft_Backend.Services
{
    public class LocalImageStorageService : IImageStorageService
    {
        public async Task<string> SaveImageAsync(IFormFile file, string folderName)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderName);

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            if (file.Length == 0) return string.Empty;

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension))
            {
                extension = file.ContentType switch
                {
                    "image/webp" => ".webp",
                    "image/png" => ".png",
                    "image/jpeg" => ".jpg",
                    _ => ".webp"
                };
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/imagenes/{folderName}/{fileName}";
        }

        public Task DeleteImageAsync(string imageUrl)
        {
            var relativePath = imageUrl.Replace("/imagenes/", "").Replace("/", Path.DirectorySeparatorChar.ToString());
            
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var physicalPath = Path.Combine(uploadsPath, relativePath);

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            return Task.CompletedTask;
        }
    }
}
