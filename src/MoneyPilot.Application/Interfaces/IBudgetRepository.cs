using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyPilot.Domain.Entities;

namespace MoneyPilot.Application.Interfaces
{
    internal interface IBudgetRepository
    {
        // Add any budget-specific methods here
        Task<Budget?> GetBudgetByUserIdAsync(string userId, int categoryId, DateTime month);
    }
}
