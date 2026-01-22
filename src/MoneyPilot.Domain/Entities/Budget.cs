using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPilot.Domain.Entities
{
    public class Budget
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        //added required keyword to Category,UserId to ensure they are not null
        public   Category ? Category { get; set; }
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public decimal MonthlyLimit { get; set; }
        public DateTime Month { get; set; }
    }
}
