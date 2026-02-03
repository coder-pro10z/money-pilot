using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPilot.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        //added required keyword to Name to ensure it is not null
        public required string Name { get; set; }
        public required ICollection<Expense> Expenses { get; set; }
        public required ICollection<Budget> Budgets { get; set; }

        // ✅ ADD THIS if you want bidirectional navigation
        public ICollection<RecurringTransaction> RecurringTransactions { get; set; }
            = new List<RecurringTransaction>();
    }
}
