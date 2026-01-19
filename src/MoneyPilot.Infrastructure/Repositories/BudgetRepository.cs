using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;
using MoneyPilot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MoneyPilot.Infrastructure.Repositories
{
    public class BudgetRepository : Repository<Budget>, IBudgetRepository
    {
        private new readonly MoneyPilotDbContext _context;

        public BudgetRepository(MoneyPilotDbContext context) : base(context)
        {
            _context = context;
        }

    


        public async Task<Budget?> GetBudgetForUserAndMonthAsync(string userId, int categoryId, DateTime month)
        {
            return await _context.Budgets
                .FirstOrDefaultAsync(b =>
                    b.UserId == userId &&
                    b.CategoryId == categoryId &&
                    b.Month.Month == month.Month &&
                    b.Month.Year == month.Year);
        }

        public async Task<Budget?> GetBudgetByUserIdAsync(string userId, int categoryId, DateTime month)
        {
            return await _context.Budgets.FirstOrDefaultAsync(b =>
                b.UserId == userId &&
                b.CategoryId == categoryId &&
                b.Month.Month == month.Month &&
                b.Month.Year == month.Year
            );
        }

        public async Task<IEnumerable<Budget>> GetBudgetsByUserIdAsync(string userId)
        {
            //return await _context.Budgets
            //    .Where(b => b.UserId == userId)
            //    .ToListAsync();
        return await _context.Budgets
        .Include(b => b.Category)
        .Include(b => b.User)
        .Where(b => b.UserId == userId)
        .ToListAsync();

        }
        // Override GetByIdAsync to include related entities
        public new async Task<Budget ?> GetByIdAsync(int id)
        {
            //Add exception handling for not found to add as Assigning a possibly-null value to a non-nullable variable or property.
            return await _context.Budgets
                .Include(b => b.Category)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id) ?? throw new InvalidOperationException("Budget not found"); ;
        }


    }
}
