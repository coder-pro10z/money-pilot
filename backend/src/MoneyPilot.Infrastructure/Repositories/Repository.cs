using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyPilot.Domain.Entities;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MoneyPilot.Infrastructure.Repositories
{
    public class Repository<T>: IRepository<T> where T : class
    {
        protected readonly MoneyPilotDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(MoneyPilotDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async  Task<T?> GetByIdAsync(int id)
        {
            //Add exception handling for not found to add as Assigning a possibly-null value to a non-nullable variable or property. 
            return await _dbSet.FindAsync(id) ?? throw new Exception("Not found");
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        //Add
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }       

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
