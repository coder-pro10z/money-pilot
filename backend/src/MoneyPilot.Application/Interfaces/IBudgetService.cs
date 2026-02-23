using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyPilot.Application.DTOs;
using MoneyPilot.Domain.Entities;

namespace MoneyPilot.Application.Interfaces
{
    public interface IBudgetService
    {
        // Add any budget-specific methods here
        Task<IEnumerable<BudgetResponseDto>> GetAllAsync(string userId);
        Task<BudgetResponseDto?> GetByIdAsync(int id, string  UserId);
        
        Task<BudgetResponseDto> AddAsync(BudgetDto dto,string userId);
        Task<bool> UpdateAsync(int id, BudgetDto dto, string UserId);
        Task<bool> DeleteAsync(int id, string userId);
    }
}
