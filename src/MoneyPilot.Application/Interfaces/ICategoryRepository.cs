using MoneyPilot.Domain.Entities;

namespace MoneyPilot.Application.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task AddAsync(Category category);
    void Update(Category category);
    void Delete(Category category);
}
