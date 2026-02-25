using Microsoft.EntityFrameworkCore;
using MoneyPilot.Application.DTOs.Dashboard;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Infrastructure.Data;

namespace MoneyPilot.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly MoneyPilotDbContext _context;

        public DashboardService(MoneyPilotDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(string userId)
        {
            // Ensure queries remain IQueryable and use AsNoTracking for performance
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

            var expensesQuery = _context.Expenses
                .AsNoTracking()
                .Include(e => e.Category)
                .Where(e => e.UserId == userId && e.Date >= startOfMonth && e.Date <= endOfMonth);

            var budgetsQuery = _context.Budgets
                .AsNoTracking()
                .Where(b => b.UserId == userId && b.Month.Year == now.Year && b.Month.Month == now.Month);

            var totalExpenses = await expensesQuery.SumAsync(e => (decimal?)e.Amount) ?? 0m;
            var totalBudget = await budgetsQuery.SumAsync(b => (decimal?)b.MonthlyLimit) ?? 0m;
            var remaining = totalBudget - totalExpenses;

            var categoryBreakdown = await expensesQuery
                .GroupBy(e => e.Category.Name)
                .Select(g => new CategoryBreakdownDto { Category = g.Key ?? "Unknown", Amount = g.Sum(x => x.Amount) })
                .ToListAsync();

            // Monthly trend - last 6 months
            var sixMonthsAgo = now.AddMonths(-5);

            var monthlyTrend = await _context.Expenses
                .AsNoTracking()
                .Where(e => e.UserId == userId && e.Date >= new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1))
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlyTrendDto
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("yyyy-MM"),
                    Amount = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            var result = new DashboardSummaryDto
            {
                TotalBudget = totalBudget,
                TotalExpenses = totalExpenses,
                RemainingBalance = remaining,
                CategoryBreakdown = categoryBreakdown,
                MonthlyTrend = monthlyTrend
            };

            return result;
        }
    }
}
