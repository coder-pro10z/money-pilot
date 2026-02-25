using MoneyPilot.Application.Interfaces;
using MoneyPilot.Infrastructure.Data;

public class UnitOfWork : IUnitofWork
{
    private readonly MoneyPilotDbContext _context;

    public IExpenseRepository Expenses { get; }
    public IBudgetRepository Budgets { get; }
    public ICategoryRepository Categories { get; }

    public UnitOfWork(MoneyPilotDbContext context,
                      IExpenseRepository expenseRepo,
                      IBudgetRepository budgetRepo,
                      ICategoryRepository categoryRepo)
    {
        _context = context;
        Expenses = expenseRepo;
        Budgets = budgetRepo;
        Categories = categoryRepo;
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
