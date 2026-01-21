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
    }
}
