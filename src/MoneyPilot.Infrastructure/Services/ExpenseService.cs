using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;

public class ExpenseService : IExpenseService
{
    private readonly IUnitofWork _unitOfWork;

    public ExpenseService(IUnitofWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ExpenseResponseDto>> GetAllAsync(string userId)
    {
        var expenses = await _unitOfWork.Expenses.GetExpensesByUserIdAsync(userId);

        return expenses.Select(e => new ExpenseResponseDto
        {
            Id = e.Id,
            Description = e.Description,
            Amount = e.Amount,
            Date = e.Date,
            CategoryId = e.CategoryId,
            CategoryName = e.Category?.Name
        });
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Expenses.GetByIdAsync(id);
    }

    public async Task AddAsync(ExpenseDto dto, string userId)
    {
        var expense = new Expense
        {
            Description = dto.Description,
            Amount = dto.Amount,
            Date = dto.Date,
            CategoryId = dto.CategoryId,
            UserId = userId
        };

        await _unitOfWork.Expenses.AddAsync(expense);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(int id, ExpenseDto dto)
    {
        var existing = await _unitOfWork.Expenses.GetByIdAsync(id);
        if (existing == null) return false;

        existing.Description = dto.Description;
        existing.Amount = dto.Amount;
        existing.CategoryId = dto.CategoryId;
        existing.Date = dto.Date;

        _unitOfWork.Expenses.Update(existing);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _unitOfWork.Expenses.GetByIdAsync(id);
        if (existing == null) return false;

        _unitOfWork.Expenses.Delete
            (existing);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
