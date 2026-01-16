using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyPilot.Domain.Entities;
namespace MoneyPilot.Application.Interfaces
{
    public interface IExpenseRepository : IRepository<Expense>
    {
        // Add any expense-specific methods here
        Task<IEnumerable<Expense>> GetExpensesByUserIdAsync(string userId);
        Task<IEnumerable<Expense>> GetExpensesByCategoryAsync(string category);
    }
}
