using LegendCraft_Backend.Data;
using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace LegendCraft_Backend.Services
{
    public class AttributeService : IAttributeService
    {
        private readonly ApplicationDbContext _context;

        public AttributeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttributeTypeResponseDto>> GetAllAttributesAsync()
        {
            // Traemos todos los tipos y sus valores activos
            return await _context.AttributeTypes
                .Include(at => at.Values)
                .Select(at => new AttributeTypeResponseDto
                {
                    Id = at.Id,
                    Name = at.Name,
                    Values = at.Values.Select(v => new AttributeValueResponseDto
                    {
                        Id = v.Id,
                        Value = v.Value
                    }).ToList()
                })
                .ToListAsync();
        }

        // --- LÓGICA DE TIPOS ---
        public async Task<int> CreateTypeAsync(AttributeTypeCreateDto dto)
        {
            var newType = new AttributeType { Name = dto.Name };
            _context.AttributeTypes.Add(newType);
            await _context.SaveChangesAsync();
            return newType.Id;
        }

        public async Task UpdateTypeAsync(int id, AttributeTypeCreateDto dto)
        {
            var type = await _context.AttributeTypes.FindAsync(id);
            if (type == null) throw new Exception("Tipo de atributo no encontrado");

            type.Name = dto.Name;
            type.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTypeAsync(int id)
        {
            var type = await _context.AttributeTypes.FindAsync(id);
            if (type == null) throw new Exception("Tipo de atributo no encontrado");

            type.IsActive = false; // Eliminación lógica
            await _context.SaveChangesAsync();
        }

        // --- LÓGICA DE VALORES ---
        public async Task<int> CreateValueAsync(int typeId, AttributeValueCreateDto dto)
        {
            var typeExists = await _context.AttributeTypes.AnyAsync(at => at.Id == typeId);
            if (!typeExists) throw new Exception("El tipo de atributo padre no existe");

            var newValue = new AttributeValue
            {
                Value = dto.Value,
                AttributeTypeId = typeId
            };

            _context.AttributeValues.Add(newValue);
            await _context.SaveChangesAsync();
            return newValue.Id;
        }

        public async Task UpdateValueAsync(int id, AttributeValueCreateDto dto)
        {
            var value = await _context.AttributeValues.FindAsync(id);
            if (value == null) throw new Exception("Valor no encontrado");

            value.Value = dto.Value;
            value.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteValueAsync(int id)
        {
            var value = await _context.AttributeValues.FindAsync(id);
            if (value == null) throw new Exception("Valor no encontrado");

            value.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
