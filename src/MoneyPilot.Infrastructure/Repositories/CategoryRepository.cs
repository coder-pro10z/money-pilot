using Microsoft.EntityFrameworkCore;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Domain.Entities;
using MoneyPilot.Infrastructure.Data;

namespace MoneyPilot.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly MoneyPilotDbContext _context;

        public CategoryRepository(MoneyPilotDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
        }
    }
}
