using MoneyPilot.Application.DTOs;

namespace MoneyPilot.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(string userId);
    Task<CategoryDto> CreateAsync(string userId, CreateCategoryDto dto);
    Task<CategoryDto> UpdateAsync(string userId, int id, CreateCategoryDto dto);
    Task DeleteAsync(string userId, int id);
}
