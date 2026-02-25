using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MoneyPilot.Domain.Entities
{
    public class AppUser: IdentityUser
    {

        //added required keyword to Expenses and Budgets to ensure they are not null
        public  ICollection<Expense> Expenses { get; set; }
        public ICollection<Budget> Budgets { get; set; }

        //Adding RecurringTransactions navigation property
        //why we need virtual keyword: to enable lazy loading if it's configured in the DbContext
        public virtual ICollection<RecurringTransaction> RecurringTransactions { get; set; }

        //intialize collections in the constructor
        public AppUser()
        {
            //why HashSet: to prevent duplicate entries and improve lookup performance
            Expenses = new HashSet<Expense>();
            Budgets = new HashSet<Budget>();
            RecurringTransactions = new HashSet<RecurringTransaction>();
        }

        
    }
}
