using Microsoft.EntityFrameworkCore;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;
using MoneyPilot.Infrastructure.Data;

namespace MoneyPilot.Infrastructure.Repositories
{
    public class ExpenseRepository : Repository<Expense>, IExpenseRepository
    {
        private readonly MoneyPilotDbContext _context;

        public ExpenseRepository(MoneyPilotDbContext context) : base(context)
        {
            _context = context;
        }

        // Get all expenses for a user
        public async Task<IEnumerable<Expense>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Expenses
                .AsNoTracking()
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        // Get single expense by Id + UserId (ownership enforced)
        public async Task<Expense?> GetByIdAsync(int id, string userId)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        }

        // Add expense
        public async Task AddAsync(Expense expense)
        {
            await _context.Expenses.AddAsync(expense);
        }

        // Update expense
        public void Update(Expense expense)
        {
            _context.Expenses.Update(expense);
        }

        // Delete expense
        public void Delete(Expense expense)
        {
            _context.Expenses.Remove(expense);
        }
    }
}
