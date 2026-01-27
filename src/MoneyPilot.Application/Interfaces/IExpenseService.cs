using MoneyPilot.Application.DTOs;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseResponseDto>> GetAllAsync(string userId);
    Task<ExpenseResponseDto?> GetByIdAsync(int id, string userId);
    Task<ExpenseResponseDto> CreateAsync(ExpenseDto dto, string userId);
    Task<bool> UpdateAsync(int id, ExpenseDto dto, string userId);
    Task<bool> DeleteAsync(int id, string userId);
}
