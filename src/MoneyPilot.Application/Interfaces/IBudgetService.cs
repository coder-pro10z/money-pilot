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
        Task<IEnumerable<Budget>> GetAllAsync(string userId);
        Task<Budget?> GetByIdAsync(int id);
        
        Task AddAsync(BudgetDto dto,String userId);
        Task<bool> UpdateAsync(int id, BudgetDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
