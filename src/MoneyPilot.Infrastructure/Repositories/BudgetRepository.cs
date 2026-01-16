using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;
using MoneyPilot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MoneyPilot.Infrastructure.Repositories
{
    public class BudgetRepository : Repository<Budget>, IBudgetRepository
    {
        private readonly MoneyPilotDbContext _context;

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


        //Task<Budget?> IBudgetRepository.GetBudgetByUserIdAsync(string userId, int categoryId, DateTime month)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
