using LegendCraft_Backend.Data;
using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace LegendCraft_Backend.Services
{
    public class BannerService : IBannerService
    {
        private readonly ApplicationDbContext _context;

        public BannerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BannerResponseDto>> GetAllBannersAsync()
        {
            return await _context.Banners
                .OrderBy(b => b.DisplayOrder)
                .Select(b => new BannerResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ImageUrl = b.ImageUrl,
                    DisplayOrder = b.DisplayOrder
                })
                .ToListAsync();
        }

        public async Task<BannerResponseDto> GetBannerByIdAsync(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) throw new Exception("Banner no encontrado");

            return new BannerResponseDto
            {
                Id = banner.Id,
                Title = banner.Title,
                ImageUrl = banner.ImageUrl,
                DisplayOrder = banner.DisplayOrder
            };
        }

        public async Task<BannerResponseDto> CreateBannerAsync(BannerCreateDto dto, Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0) throw new Exception("Debe proporcionar una imagen para el banner.");

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "banners");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var banner = new Banner
            {
                Title = dto.Title,
                DisplayOrder = dto.DisplayOrder,
                ImageUrl = $"/imagenes/banners/{fileName}"
            };

            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();

            return new BannerResponseDto
            {
                Id = banner.Id,
                Title = banner.Title,
                ImageUrl = banner.ImageUrl,
                DisplayOrder = banner.DisplayOrder
            };
        }

        public async Task UpdateBannerAsync(int id, BannerUpdateDto dto, Microsoft.AspNetCore.Http.IFormFile? file)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) throw new Exception("Banner no encontrado");

            banner.Title = dto.Title;
            banner.DisplayOrder = dto.DisplayOrder;

            if (file != null && file.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "banners");
                if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Delete old image if it exists
                if (!string.IsNullOrEmpty(banner.ImageUrl))
                {
                    var oldFileName = banner.ImageUrl.Split('/').LastOrDefault();
                    if (oldFileName != null)
                    {
                        var oldPath = Path.Combine(uploadsPath, oldFileName);
                        if (File.Exists(oldPath)) File.Delete(oldPath);
                    }
                }

                banner.ImageUrl = $"/imagenes/banners/{fileName}";
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBannerAsync(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) throw new Exception("Banner no encontrado");

            banner.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
