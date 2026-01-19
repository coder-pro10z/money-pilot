using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPilot.Application.Interfaces
{
    public interface IRepository<T> where T:class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T Entity);
        void Update(T Entity);
        void Delete(T Entity);
    }
}
