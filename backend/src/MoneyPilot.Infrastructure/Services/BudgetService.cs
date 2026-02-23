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
             //CategoryName  = s.Category?.Name ?? "Unknown"
    });
        }

        public async Task<BudgetResponseDto?> GetByIdAsync(int id ,string userId)
        {
            var budget =  await _unitOfWork.Budgets.GetByIdAsync(id);
            if (budget == null || budget.UserId != userId)
                return null;

                return new BudgetResponseDto
            {
                Id = budget.Id,
                MonthlyLimit = budget.MonthlyLimit,
                Month = budget.Month,
                CategoryId = budget.CategoryId,
                //CategoryName  = s.Category?.Name ?? "Unknown"
            };
        }

        public async Task<BudgetResponseDto> AddAsync(BudgetDto dto, string userId)
        {
            // Fetch the required Category entity to satisfy the required 'Category' property
            //var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            //if (category == null)
            //{
            //    throw new InvalidOperationException($"Category with ID {dto.CategoryId} not found.");
            //}

            var budget = new Budget
            {
                MonthlyLimit = dto.MonthlyLimit,
                Month = dto.Month,
                CategoryId = dto.CategoryId,
                //Category = category, // Set required property
                UserId = userId
            };

            await _unitOfWork.Budgets.AddAsync(budget);
            await _unitOfWork.SaveChangesAsync();
            return new BudgetResponseDto
            {
                Id = budget.Id,
                MonthlyLimit = budget.MonthlyLimit,
                Month = budget.Month,
                CategoryId = budget.CategoryId,
                //CategoryName  = s.Category?.Name ?? "Unknown"
            };
        }

        public async Task<bool> UpdateAsync(int id, BudgetDto dto, string userId)
        {
            var existing = await _unitOfWork.Budgets.GetByIdAsync(id);
            if (existing == null||existing.UserId!= userId) return false;

            

            existing.MonthlyLimit = dto.MonthlyLimit;
            existing.Month = dto.Month;
            existing.CategoryId = dto.CategoryId;

            _unitOfWork.Budgets.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var existing = await _unitOfWork.Budgets.GetByIdAsync(id);
            if (existing == null || existing.UserId!=userId) return false;

            _unitOfWork.Budgets.Delete(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        //public Task<BudgetResponseDto> GetByIdAsync(string UserId)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<BudgetResponseDto> IBudgetService.AddAsync(BudgetDto dto, string userId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
