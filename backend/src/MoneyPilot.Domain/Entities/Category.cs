using System.Collections.Generic;

namespace MoneyPilot.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Color { get; set; }

        // Remove 'required' keyword and initialize with empty collections
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();

        // Optional: Add RecurringTransactions if needed
        public ICollection<RecurringTransaction> RecurringTransactions { get; set; } = new List<RecurringTransaction>();
        public bool IsDeleted { get; set; }
    }
}