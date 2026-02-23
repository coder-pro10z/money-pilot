using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;

public interface IExpenseRepository : IRepository<Expense>
{
    Task<IEnumerable<Expense>> GetAllByUserIdAsync(string userId);
    Task<Expense?> GetByIdAsync(int id, string userId);
    Task AddAsync(Expense expense);
    void Update(Expense expense);
    void Delete(Expense expense);
}
