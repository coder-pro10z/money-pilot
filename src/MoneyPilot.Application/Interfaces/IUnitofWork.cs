using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPilot.Application.Interfaces
{
    public interface IUnitofWork
    {
        IExpenseRepository Expenses { get; }
        IBudgetRepository Budgets { get; }
        Task<int> SaveChangesAsync(); // wraps DbContext.SaveChangesAsync()
    }
}
