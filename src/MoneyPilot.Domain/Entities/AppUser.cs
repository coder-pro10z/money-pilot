using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPilot.Domain.Entities
{
    public class AppUser
    {
        public required string Id { get; set; }
        public required string Email { get; set; }

        //added required keyword to Expenses and Budgets to ensure they are not null
        public required ICollection<Expense> Expenses { get; set; }
        public required ICollection<Budget> Budgets { get; set; }
    }
}
