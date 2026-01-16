using MoneyPilot.Application.Interfaces;
using MoneyPilot.Infrastructure.Data;

public class UnitOfWork : IUnitofWork
{
    private readonly MoneyPilotDbContext _context;

    public IExpenseRepository Expenses { get; }
    public IBudgetRepository Budgets { get; }

    public UnitOfWork(MoneyPilotDbContext context,
                      IExpenseRepository expenseRepo,
                      IBudgetRepository budgetRepo)
    {
        _context = context;
        Expenses = expenseRepo;
        Budgets = budgetRepo;
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
