using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;

namespace MoneyPilot.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitofWork _unitOfWork;

        public CategoryService(IUnitofWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(string userId)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Color = c.Color
            });
        }

        public async Task<CategoryDto> CreateAsync(string userId, CreateCategoryDto dto)
        {
            var  category= new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = dto.Color
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Color = category.Color
            };
        }

        public async Task<CategoryDto> UpdateAsync(string userId, int id, CreateCategoryDto dto)
        {
            var existing = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Category not found");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Color = dto.Color;

            _unitOfWork.Categories.Update(existing);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryDto
            {
                Id = existing.Id,
                Name = existing.Name,
                Description = existing.Description,
                Color = existing.Color
            };
        }

        public async Task DeleteAsync(string userId, int id)
        {
            var existing = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Category not found");

            // Soft delete
            existing.IsDeleted = true;
            _unitOfWork.Categories.Update(existing);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
