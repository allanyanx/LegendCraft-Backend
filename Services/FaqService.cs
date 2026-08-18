using LegendCraft_Backend.Data;
using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace LegendCraft_Backend.Services
{
    public class FaqService : IFaqService
    {
        private readonly ApplicationDbContext _context;

        public FaqService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FaqResponseDto>> GetAllFaqsAsync()
        {
            return await _context.Faqs
                .OrderBy(f => f.DisplayOrder)
                .Select(f => new FaqResponseDto
                {
                    Id = f.Id,
                    Question = f.Question,
                    Answer = f.Answer,
                    DisplayOrder = f.DisplayOrder
                })
                .ToListAsync();
        }

        public async Task<FaqResponseDto> GetFaqByIdAsync(int id)
        {
            var faq = await _context.Faqs.FindAsync(id);
            if (faq == null) throw new Exception("FAQ no encontrada");

            return new FaqResponseDto
            {
                Id = faq.Id,
                Question = faq.Question,
                Answer = faq.Answer,
                DisplayOrder = faq.DisplayOrder
            };
        }

        public async Task<FaqResponseDto> CreateFaqAsync(FaqCreateDto dto)
        {
            var faq = new Faq
            {
                Question = dto.Question,
                Answer = dto.Answer,
                DisplayOrder = dto.DisplayOrder
            };

            _context.Faqs.Add(faq);
            await _context.SaveChangesAsync();

            return new FaqResponseDto
            {
                Id = faq.Id,
                Question = faq.Question,
                Answer = faq.Answer,
                DisplayOrder = faq.DisplayOrder
            };
        }

        public async Task UpdateFaqAsync(int id, FaqUpdateDto dto)
        {
            var faq = await _context.Faqs.FindAsync(id);
            if (faq == null) throw new Exception("FAQ no encontrada");

            faq.Question = dto.Question;
            faq.Answer = dto.Answer;
            faq.DisplayOrder = dto.DisplayOrder;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteFaqAsync(int id)
        {
            var faq = await _context.Faqs.FindAsync(id);
            if (faq == null) throw new Exception("FAQ no encontrada");

            faq.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
