using MoneyPilot.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoneyPilot.Application.Interfaces
{
    public interface IRecurringTransactionService
    {
        Task<RecurringTransactionDto> GetByIdAsync(int id, string userId);
        Task<IEnumerable<RecurringTransactionDto>> GetAllAsync(string userId);
        Task<RecurringTransactionDto> CreateAsync(CreateRecurringTransactionDto dto, string userId);
        Task<RecurringTransactionDto> UpdateAsync(int id, UpdateRecurringTransactionDto dto, string userId);
        Task DeleteAsync(int id, string userId);
        Task<int> ProcessDueTransactionsAsync();
        Task<IEnumerable<RecurringTransactionDto>> GetDueTransactionsAsync(string userId);
    }
}
