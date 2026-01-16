using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;
using MoneyPilot.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MoneyPilot.Infrastructure.Repositories
{

        public class ExpenseRepository : Repository<Expense>, IExpenseRepository
        {
            private readonly MoneyPilotDbContext _context;

            public ExpenseRepository(MoneyPilotDbContext context) : base(context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Expense>> GetExpensesByUserIdAsync(string userId)
            {
                return await _context.Expenses
                    .Where(e => e.UserId == userId)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Expense>> GetExpensesByCategoryAsync(int categoryId)
            {
                return await _context.Expenses
                    .Where(e => e.CategoryId == categoryId)
                    .ToListAsync();
            }

    }
    }

