using LegendCraft_Backend.DTOs;

namespace LegendCraft_Backend.Services
{
    public interface IAttributeService
    {
        // Lectura del árbol completo
        Task<IEnumerable<AttributeTypeResponseDto>> GetAllAttributesAsync();

        // Gestión de Tipos (Ej: "Categoría", "Marca")
        Task<int> CreateTypeAsync(AttributeTypeCreateDto dto);
        Task UpdateTypeAsync(int id, AttributeTypeCreateDto dto);
        Task DeleteTypeAsync(int id);

        // Gestión de Valores (Ej: "Teclados", "Ratones" dentro de Categoría)
        Task<int> CreateValueAsync(int typeId, AttributeValueCreateDto dto);
        Task UpdateValueAsync(int id, AttributeValueCreateDto dto);
        Task DeleteValueAsync(int id);
    }
}
