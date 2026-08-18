using LegendCraft_Backend.DTOs;

namespace LegendCraft_Backend.Services
{
    public interface IFaqService
    {
        Task<List<FaqResponseDto>> GetAllFaqsAsync();
        Task<FaqResponseDto> GetFaqByIdAsync(int id);
        Task<FaqResponseDto> CreateFaqAsync(FaqCreateDto dto);
        Task UpdateFaqAsync(int id, FaqUpdateDto dto);
        Task DeleteFaqAsync(int id);
    }
}
