using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace LegendCraft_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributesController : ControllerBase
    {
        private readonly IAttributeService _attributeService;

        public AttributesController(IAttributeService attributeService)
        {
            _attributeService = attributeService;
        }

        // Obtiene todo el árbol para pintar la UI
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var attributes = await _attributeService.GetAllAttributesAsync();
            return Ok(attributes);
        }

        // --- ENDPOINTS PARA TIPOS (Ej: Marca) ---

        [HttpPost("types")]
        public async Task<IActionResult> CreateType([FromBody] AttributeTypeCreateDto dto)
        {
            var newId = await _attributeService.CreateTypeAsync(dto);
            return Ok(new { Message = "Tipo creado", Id = newId });
        }

        [HttpPut("types/{id}")]
        public async Task<IActionResult> UpdateType(int id, [FromBody] AttributeTypeCreateDto dto)
        {
            await _attributeService.UpdateTypeAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("types/{id}")]
        public async Task<IActionResult> DeleteType(int id)
        {
            await _attributeService.DeleteTypeAsync(id);
            return NoContent();
        }

        // --- ENDPOINTS PARA VALORES (Ej: Logitech, Corsair) ---

        [HttpPost("types/{typeId}/values")]
        public async Task<IActionResult> CreateValue(int typeId, [FromBody] AttributeValueCreateDto dto)
        {
            var newId = await _attributeService.CreateValueAsync(typeId, dto);
            return Ok(new { Message = "Valor creado", Id = newId });
        }

        [HttpPut("values/{id}")]
        public async Task<IActionResult> UpdateValue(int id, [FromBody] AttributeValueCreateDto dto)
        {
            await _attributeService.UpdateValueAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("values/{id}")]
        public async Task<IActionResult> DeleteValue(int id)
        {
            await _attributeService.DeleteValueAsync(id);
            return NoContent();
        }
    }
}
