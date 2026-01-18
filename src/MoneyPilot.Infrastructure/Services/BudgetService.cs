using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;

namespace MoneyPilot.Infrastructure.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IUnitofWork _unitOfWork;

        public BudgetService(IUnitofWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BudgetResponseDto>> GetAllAsync(string userId)
        {
            var budgets=  await _unitOfWork.Budgets.GetBudgetsByUserIdAsync(userId);
            return budgets.Select(s=> new BudgetResponseDto
            {
             Id = s.Id,
             MonthlyLimit =s.MonthlyLimit,  
             Month  = s.Month,
             CategoryId  = s.CategoryId,
             CategoryName  = s.Category.Name
    });
        }

        public async Task<Budget?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Budgets.GetByIdAsync(id);
        }

        public async Task AddAsync(BudgetDto dto, string userId)
        {
            var budget = new Budget
            {
                MonthlyLimit = dto.MonthlyLimit,
                Month = dto.Month,
                CategoryId = dto.CategoryId,
                UserId = userId
            };

            await _unitOfWork.Budgets.AddAsync(budget);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, BudgetDto dto)
        {
            var existing = await _unitOfWork.Budgets.GetByIdAsync(id);
            if (existing == null) return false;

            existing.MonthlyLimit = dto.MonthlyLimit;
            existing.Month = dto.Month;
            existing.CategoryId = dto.CategoryId;

            _unitOfWork.Budgets.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Budgets.GetByIdAsync(id);
            if (existing == null) return false;

            _unitOfWork.Budgets.Delete(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
