using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;

namespace MoneyPilot.Infrastructure.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUnitofWork _unitOfWork;

        public ExpenseService(
            IExpenseRepository expenseRepository,
            IUnitofWork unitOfWork)
        {
            _expenseRepository = expenseRepository;
            _unitOfWork = unitOfWork;
        }

        // ===================== GET ALL =====================
        public async Task<IEnumerable<ExpenseResponseDto>> GetAllAsync(string userId)
        {
            var expenses = await _expenseRepository.GetAllByUserIdAsync(userId);

            return expenses.Select(e => new ExpenseResponseDto
            {
                Id = e.Id,
                Description = e.Description,
                Amount = e.Amount,
                CategoryId = e.CategoryId,
                CategoryName = e.Category?.Name,
                Date = e.Date
            });
        }

        // ===================== GET BY ID =====================
        public async Task<ExpenseResponseDto?> GetByIdAsync(int id, string userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id, userId);

            if (expense == null)
                return null;

            return new ExpenseResponseDto
            {
                Id = expense.Id,
                Description = expense.Description,
                Amount = expense.Amount,
                CategoryId = expense.CategoryId,
                CategoryName = expense.Category?.Name,
                Date = expense.Date
            };
        }

        // ===================== CREATE =====================
        public async Task CreateAsync(ExpenseDto dto, string userId)
        {
            var expense = new Expense
            {
                Description = dto.Description,
                Amount = dto.Amount,
                CategoryId = dto.CategoryId,
                Date = dto.Date,
                UserId = userId // 🔐 JWT-derived, never from client
            };

            await _expenseRepository.AddAsync(expense);
            await _unitOfWork.SaveChangesAsync();
        }

        // ===================== UPDATE =====================
        public async Task<bool> UpdateAsync(int id, ExpenseDto dto, string userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id, userId);

            if (expense == null)
                return false;

            expense.Description = dto.Description;
            expense.Amount = dto.Amount;
            expense.CategoryId = dto.CategoryId;
            expense.Date = dto.Date;

            _expenseRepository.Update(expense);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // ===================== DELETE =====================
        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id, userId);

            if (expense == null)
                return false;

            _expenseRepository.Delete(expense);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
