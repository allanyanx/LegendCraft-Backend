using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LegendCraft_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticlesController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleCreateDto dto)
        {
            // Validamos que el JSON no venga nulo o con errores básicos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Llamamos al servicio
            var newArticleId = await _articleService.CreateArticleAsync(dto);

            // Respondemos con un 201 Created y el ID del nuevo artículo
            return StatusCode(201, new { Message = "Artículo creado con éxito", ArticleId = newArticleId });
        }

        [HttpPost("{articleId}/images")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImages(int articleId, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No se enviaron imágenes.");
            }

            // Llamamos al servicio para manejar el guardado físico y la base de datos
            var savedImages = await _articleService.SaveImagesAsync(articleId, files);

            return Ok(new { Message = "Imágenes subidas con éxito", Images = savedImages });
        }

        [HttpPut("{articleId}/images/{imageId}/set-main")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetMainImage(int articleId, int imageId)
        {
            try
            {
                await _articleService.SetMainImageAsync(articleId, imageId);
                return Ok(new { Message = "Imagen principal actualizada con éxito." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllArticles(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] List<int>? attributeValues = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? sortBy = "relevantes",
        [FromQuery] bool? isPrintOnDemand = null,
        [FromQuery] bool? isOnSale = null)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;

            var result = await _articleService.GetAllArticlesAsync(pageNumber, pageSize, search, attributeValues, maxPrice, sortBy, isPrintOnDemand, isOnSale);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);

            if (article == null)
            {
                return NotFound(new { Message = "El artículo solicitado no existe." });
            }

            return Ok(article);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] ArticleUpdateDto dto)
        {
            try
            {
                await _articleService.UpdateArticleAsync(id, dto);
                return NoContent(); // 204 No Content es el estándar REST cuando un PUT es exitoso
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            try
            {
                await _articleService.DeleteArticleAsync(id);
                return NoContent(); // Devolvemos 204 para indicar que se procesó correctamente
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{articleId}/images/{imageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImage(int articleId, int imageId)
        {
            try
            {
                await _articleService.DeleteImageAsync(articleId, imageId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
