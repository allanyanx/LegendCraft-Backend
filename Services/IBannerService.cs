using LegendCraft_Backend.DTOs;

namespace LegendCraft_Backend.Services
{
    public interface IBannerService
    {
        Task<List<BannerResponseDto>> GetAllBannersAsync();
        Task<BannerResponseDto> GetBannerByIdAsync(int id);
        Task<BannerResponseDto> CreateBannerAsync(BannerCreateDto dto, Microsoft.AspNetCore.Http.IFormFile file);
        Task UpdateBannerAsync(int id, BannerUpdateDto dto, Microsoft.AspNetCore.Http.IFormFile? file);
        Task DeleteBannerAsync(int id);
    }
}
