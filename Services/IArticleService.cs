using LegendCraft_Backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LegendCraft_Backend.Services
{
    public interface IArticleService
    {
        Task<int> CreateArticleAsync(ArticleCreateDto dto);
        Task<List<string>> SaveImagesAsync(int articleId, [FromForm] List<IFormFile> files);
        Task SetMainImageAsync(int articleId, int imageId);
        Task<PagedResultDto<ArticleListResponseDto>> GetAllArticlesAsync(int pageNumber, int pageSize);
        Task<ArticleDetailResponseDto?> GetArticleByIdAsync(int id);
        Task UpdateArticleAsync(int id, ArticleUpdateDto dto);
        Task DeleteArticleAsync(int id);
        Task DeleteImageAsync(int articleId, int imageId);
    }
}
