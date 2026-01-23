using MoneyPilot.Application.DTOs;
using MoneyPilot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPilot.Application.Interfaces
{
    public interface IExpenseService
    {
 Task<IEnumerable<ExpenseResponseDto>> GetAllAsync(string userId);
Task<ExpenseResponseDto?> GetByIdAsync(int id, string userId);
Task CreateAsync(ExpenseDto dto, string userId);
Task<bool> UpdateAsync(int id, ExpenseDto dto, string userId);
Task<bool> DeleteAsync(int id, string userId);

    }

}

